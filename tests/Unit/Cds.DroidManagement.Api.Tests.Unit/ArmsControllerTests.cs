using Cds.DroidManagement.Api.DroidFeature;
using Cds.DroidManagement.Api.DroidFeature.ViewModels;
using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Domain.DroidAggregate.Commands;
using Cds.DroidManagement.Domain.DroidAggregate.Exceptions;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Abstractions;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cds.DroidManagement.Api.Tests.Unit
{
    public class ArmsControllerTests
    {
        private readonly Mock<IReadArmRepository> _repo;
        private readonly Mock<IReadDroidRepository> _repoDroid;
        private readonly Mock<IDroidHandler> _handler;
        private readonly ArmsController _controller;

        public ArmsControllerTests()
        {
            _repo = new Mock<IReadArmRepository>();
            _repoDroid = new Mock<IReadDroidRepository>();
            _handler = new Mock<IDroidHandler>();
            _controller = new ArmsController(_repo.Object, _repoDroid.Object, _handler.Object);
        }

        [Fact]
        public async Task CreateArm_WithValidSerialNumber_ReturnsNoContentStatusCodeAsync()
        {
            // Arrange
            var serialNumber = Guid.Parse("341d9e5b-ae06-4c06-ba4f-7256317792cd");
            var createArm = new CreateArm(serialNumber);

            _handler
                .Setup(x => x.HandleAsync(createArm))
                .Returns(Task.CompletedTask);

            // Act
            var httpResponse = await _controller.PostAsync(serialNumber);
            var httpCode = ((StatusCodeResult)httpResponse).StatusCode;

            // Assert
            httpCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GetArmForDroid_WithValidDroidId_ReturnsDroidArmListAsync()
        {
            // Arrange
            var serialNumber = Guid.Parse("7e202cf3-d50f-4d70-bab9-c0f3a35c5bb2");
            IReadOnlyCollection<IArmDto> arms = new List<IArmDto>
            {
                new ArmDto
                {
                    DroidId = serialNumber,
                    ArmId = Guid.Parse("e863e570-4516-4966-8920-93cd5c9a6e3e")
                },
                new ArmDto
                {
                    DroidId = serialNumber,
                    ArmId = Guid.Parse("cffc1357-6b1b-49b0-8020-c59d22de9162")
                }
            };
            IDroidDto droidDto = new DroidDto
            {
                DroidId = Guid.Parse("7e202cf3-d50f-4d70-bab9-c0f3a35c5bb2"),
                CreationDate = DateTimeOffset.Now,
                Name = "test",
                Nickname = "test",
                Quote = "quote"
            };

            _repo
                .Setup(x => x.GetDroidArmsAsync(serialNumber))
                .Returns(Task.FromResult(arms));

            _repoDroid
                .Setup(r => r.GetBySerialNumberAsync(serialNumber, It.IsAny<Action<IDroidUnicityValidationInfo>>()))
                .Returns(Task.FromResult(droidDto));

            // Act
            var httpResponse = await _controller.GetAsync(serialNumber);
            var result = (ObjectResult)httpResponse.Result;
            var armsResult = (ICollection<ArmViewModel>)result.Value;

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            armsResult.Should().HaveCount(arms.Count);
            var armIds = arms.Select(a => a.ArmId);
            var armIdsResult = armsResult.Select(a => a.SerialNumber);
            var allSame = armIds.All(armId => armIdsResult.Contains(armId));
            allSame.Should().BeTrue();
        }

        [Fact]
        public async Task GetArmForDroid_WithNonexistentDroid_ReturnsNotFoundStatusCodeAsync()
        {
            // Arrange
            var serialNumber = Guid.Parse("7e202cf3-d50f-4d70-bab9-c0f3a35c5bb2");

            _repo
                .Setup(x => x.GetDroidArmsAsync(serialNumber));

            _repoDroid
                .Setup(r => r.GetBySerialNumberAsync(serialNumber, It.IsAny<Action<IDroidUnicityValidationInfo>>()))
                .Throws<DroidNotFoundException>();

            // Act
            var httpResponse = await _controller.GetAsync(serialNumber);

            // Assert
            ((StatusCodeResult)httpResponse.Result).StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateArm_WithNonexistentDroid_ReturnsNotFoundStatusCodeAsync()
        {
            // Arrange
            var serialNumber = Guid.Parse("341d9e5b-ae06-4c06-ba4f-7256317792cd");

            _handler
                .Setup(x => x.HandleAsync(It.IsAny<CreateArm>()))
                .Throws<DroidNotFoundException>();

            // Act
            var httpResponse = await _controller.PostAsync(serialNumber);
            var httpCode = ((StatusCodeResult)httpResponse).StatusCode;

            // Assert
            httpCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
