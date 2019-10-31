using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cds.DroidManagement.Api.DroidFeature;
using Cds.DroidManagement.Api.DroidFeature.ViewModels;
using Cds.DroidManagement.Domain.DroidAggregate;
using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Domain.DroidAggregate.Commands;
using Cds.DroidManagement.Domain.DroidAggregate.Exceptions;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Abstractions;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Cds.DroidManagement.Api.Tests.Unit
{
    public class DroidsControllerTests
    {
        private readonly Mock<IReadDroidRepository> _repo;
        private readonly Mock<IDroidHandler> _handler;
        private readonly DroidsController _controller;        

        public DroidsControllerTests()
        {
            _repo = new Mock<IReadDroidRepository>();
            _handler = new Mock<IDroidHandler>();
            _controller = new DroidsController(_repo.Object, _handler.Object);            
        }

        [Fact]
        public async Task GetPagedDroid_WithoutPagination_Returns10ElementsAsync()
        {
            // Arrange
            var droids = new List<IDroidDto> { new DroidDto() };

            _repo
                .Setup(x => x.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(((IReadOnlyCollection<IDroidDto>)droids, true)));


            // Act
            var httpResponse = await _controller.GetAsync();
            var result = (ObjectResult)httpResponse.Result;
            var pagedList = (PagedList<DroidViewModel>)result.Value;

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            pagedList.Items.Should().HaveCount(1);
            pagedList.HasNextPage.Should().BeTrue();
        }

        [Fact]
        public async Task GetPagedDroid_WithPagination_ReturnsDroidListAsync()
        {
            // Arrange
            var droids = new List<IDroidDto>
            {
                new DroidDto
                {
                    DroidId = Guid.Parse("341d9e5b-ae06-4c06-ba4f-7256317792cd"),
                    CreationDate = new DateTime(2019, 02, 06),
                    Name = "Toto",
                    Nickname = "To"
                },
                new DroidDto
                {
                    DroidId = Guid.Parse("c50e2592-0a71-4ff6-90ce-052cca08598d"),
                    CreationDate = new DateTime(2019, 02, 07),
                    Name = "Tata",
                    Nickname = "Ta"
                }
            };
            var droidResult = (IReadOnlyCollection<IDroidDto>)new List<IDroidDto> { droids[0] };

            _repo
                .Setup(x => x.GetAllPagedAsync(0, 1))
                .Returns(Task.FromResult((droidResult, true)));

            // Act
            var httpResponse = await _controller.GetAsync(0, 1);
            var result = (ObjectResult)httpResponse.Result;
            var pagedList = (PagedList<DroidViewModel>)result.Value;

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            pagedList.Items.Should().HaveCount(1);
            pagedList.PageIndex.Should().Be(0);
            pagedList.PageSize.Should().Be(1);
            pagedList.HasNextPage.Should().BeTrue();
            var returnedSerialNumber = pagedList.Items.ToArray()[0].SerialNumber;
            returnedSerialNumber.Should().Be(droids[0].DroidId);
        }

        [Fact]
        public async Task GetDroid_WithValidSerialNumber_ReturnsDroidAsync()
        {
            // Arrange            
            IDroidDto droid = new DroidDto
            {
                DroidId = Guid.Parse("341d9e5b-ae06-4c06-ba4f-7256317792cd"),
                CreationDate = new DateTime(2019, 02, 06),
                Name = "Toto",
                Nickname = "To"
            };

            _repo
                .Setup(x => x.GetBySerialNumberAsync(droid.DroidId, Droid.AssertExists))
                .Returns(Task.FromResult(droid));

            // Act
            var httpResponse = await _controller.GetAsync(droid.DroidId);
            var result = (ObjectResult)httpResponse.Result;
            var element = (DroidViewModel)result.Value;

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            element.SerialNumber.Should().Be(droid.DroidId);
            element.Name.Should().Be(droid.Name);
            element.Nickname.Should().Be(droid.Nickname);
        }

        [Fact]
        public async Task GetDroid_WithNonexistentSerialNumber_ReturnsNotFoundStatusCodeAsync()
        {
            // Arrange
            var serialNumber = Guid.Parse("341d9e5b-ae06-4c06-ba4f-7256317792cd");
            _repo
                .Setup(r => r.GetBySerialNumberAsync(serialNumber, It.IsAny<Action<IDroidUnicityValidationInfo>>()))
                .Throws<DroidNotFoundException>();

            // Act
            var httpResponse = await _controller.GetAsync(serialNumber);

            // Assert
            ((StatusCodeResult)httpResponse.Result).StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task InsertDroid_WithValidDroid_ReturnsEqualDroidAsync()
        {
            // Arrange
            var createDroid = new CreateDroid("Toto", "To");
            var droidDto = new DroidDto
            {
                DroidId = Guid.Parse("341d9e5b-ae06-4c06-ba4f-7256317792cd"),
                CreationDate = new DateTime(2019, 02, 06),
                Name = "Toto",
                Nickname = "To"
            };
            var armDtoList = new List<IArmDto>
            {
                new ArmDto { DroidId = droidDto.DroidId, ArmId = Guid.NewGuid() }
            };            

            var droid = Droid.FromDto(droidDto).WithArms(armDtoList);

            _handler
                .Setup(x => x.HandleAsync(createDroid))
                .Returns(Task.FromResult(droid));

            // Act
            var httpResponse = await _controller.PostAsync(createDroid);
            var result = (ObjectResult)httpResponse;
            var element = (DroidViewModel)result.Value;

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);
            element.SerialNumber.Should().Be(droidDto.DroidId);
            element.Name.Should().Be(droidDto.Name);
            element.Nickname.Should().Be(droidDto.Nickname);
        }

        [Fact]
        public async Task UpdateDroid_WithValidDroid_ReturnsNoContentStatusCodeAsync()
        {
            // Arrange
            var serialNumber = Guid.Parse("341d9e5b-ae06-4c06-ba4f-7256317792cd");
            var updateDroid = new UpdateDroid("Toto", "To", serialNumber);

            _handler
                .Setup(x => x.HandleAsync(updateDroid))
                .Returns(Task.CompletedTask);

            // Act
            var httpResponse = await _controller.PutAsync(serialNumber, updateDroid);
            var httpCode = ((StatusCodeResult)httpResponse).StatusCode;

            // Assert
            httpCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteDroid_WithValidSerialNumber_ReturnsNoContentStatusCodeAsync()
        {
            // Arrange
            var serialNumber = Guid.Parse("341d9e5b-ae06-4c06-ba4f-7256317792cd");

            _handler
                .Setup(x => x.HandleAsync(It.IsAny<DeleteDroid>()))
                .Returns(Task.FromResult(true));

            // Act
            var httpResponse = await _controller.DeleteAsync(serialNumber);
            var httpCode = ((StatusCodeResult)httpResponse).StatusCode;

            // Assert
            httpCode.Should().Be((int)HttpStatusCode.NoContent);
        }
        
        [Fact]
        public async Task InsertDroid_WithInvalidDroidName_ReturnsConflictStatusCodeAsync()
        {
            // Arrange
            var createDroid = new CreateDroid("Toto", "To");
            _handler
                .Setup(x => x.HandleAsync(createDroid))
                .Throws<DroidConflictNameException>();

            // Act
            var httpResponse = await _controller.PostAsync(createDroid);
            var httpCode = ((StatusCodeResult)httpResponse).StatusCode;

            // Assert
            httpCode.Should().Be((int)HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task UpdateDroid_WithInvalidDroidName_ReturnsConflictStatusCodeAsync()
        {
            // Arrange
            var serialNumber = Guid.Parse("341d9e5b-ae06-4c06-ba4f-7256317792cd");
            var updateDroid = new UpdateDroid("Toto", "To", serialNumber);

            _handler
                .Setup(x => x.HandleAsync(updateDroid))
                .Throws<DroidConflictNameException>();

            // Act
            var httpResponse = await _controller.PutAsync(serialNumber, updateDroid);
            var httpCode = ((StatusCodeResult)httpResponse).StatusCode;

            // Assert
            httpCode.Should().Be((int)HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task DeleteDroid_WithInexistentDroidId_ReturnsNotFoundStatusCodeAsync()
        {
            // Arrange
            var serialNumber = Guid.Parse("341d9e5b-ae06-4c06-ba4f-7256317792cd");

            _handler
                .Setup(x => x.HandleAsync(It.IsAny<DeleteDroid>()))
                .Throws<DroidNotFoundException>();

            // Act
            var httpResponse = await _controller.DeleteAsync(serialNumber);
            var httpCode = ((StatusCodeResult)httpResponse).StatusCode;

            // Assert
            httpCode.Should().Be((int)HttpStatusCode.NotFound);
        }        
    }
}
