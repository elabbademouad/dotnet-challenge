using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Cds.DroidManagement.Domain.DroidAggregate;
using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Domain.DroidAggregate.Commands;
using Cds.DroidManagement.Domain.DroidAggregate.Exceptions;
using Cds.DroidManagement.Domain.DroidAggregate.RepositoryAction;
using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos;
using FluentAssertions;
using Moq;
using Xunit;

namespace Cds.DroidManagement.Domain.Tests.Unit
{
    public class DroidHandlerTests
    {
        private readonly Mock<IDroidRepository> _repoMock;
        private readonly Mock<IEncryptionService> _encryptionMock;
        private readonly Mock<IArmRepository> _repoArmMock;
        private readonly Mock<IDroidQuotesService> _quoteMock;
        private readonly Guid _serialNumber;
        private readonly IDroidDto _droidDto;
        private readonly IReadOnlyCollection<IArmDto> _armDtoList;

        public DroidHandlerTests()
        {
            _encryptionMock = new Mock<IEncryptionService>();
            _repoMock = new Mock<IDroidRepository>();
            _repoMock
                .Setup(repo => repo.InsertAsync(It.IsAny<Droid>()))
                .Returns(Task.CompletedTask);
            _repoMock
                .Setup(repo => repo.DoesNameAlreadyExistsAsync(It.IsAny<DroidName>()))
                .Returns(Task.FromResult(false));

            _repoArmMock = new Mock<IArmRepository>();

            _quoteMock = new Mock<IDroidQuotesService>();
            _quoteMock.Setup(repo => repo.GetRandomQuoteAsync())
                .Returns(Task.FromResult("The Force will be with you. Always. — Obi-Wan Kenobi"));

            _serialNumber = new Guid("c29a188b-a562-47b4-a74c-9e82702e0986");
            _droidDto = new DroidDto
            {
                DroidId = _serialNumber,
                CreationDate = DateTimeOffset.Now,
                Name = "Toto",
                Nickname = "To",
            };
            _armDtoList = new List<IArmDto>
            {
                new ArmDto { DroidId = _serialNumber, ArmId = Guid.NewGuid() }
            };
        }

        [Fact]
        public async Task HandleCreateDroid_WithValidDroid_DroidCreatedSuccessfully()
        {
            // Arrange
            var droidHandler = new DroidHandler(_repoMock.Object, _repoArmMock.Object, _quoteMock.Object, _encryptionMock.Object);
            var createDroid = new CreateDroid("Toto", "To");

            // Act
            var droidCreated = await droidHandler.HandleAsync(createDroid).ConfigureAwait(false);

            // Assert
            _repoMock.Verify((r) => r.DoesNameAlreadyExistsAsync(createDroid.Name), Times.Once);
            _repoMock.Verify((r) => r.InsertAsync(droidCreated), Times.Once);

            (droidCreated.Name).Should().Be(createDroid.Name);
            (droidCreated.Nickname).Should().Be(createDroid.Nickname);
        }

        [Fact]
        public async Task HandleUpdateDroid_WithValidName_DroidUpdatedSuccessfully()
        {
            // Arrange
            var droidHandler = new DroidHandler(_repoMock.Object, _repoArmMock.Object, _quoteMock.Object, _encryptionMock.Object);
            var updateDroid = new UpdateDroid("Toto", "To", _serialNumber);
            IReadOnlyCollection<IArmDto> armDtoList = new List<ArmDto>
            {
                new ArmDto { DroidId = _serialNumber, ArmId = Guid.NewGuid() },
                new ArmDto { DroidId = _serialNumber, ArmId = Guid.NewGuid() }
            };

            var getDroidResult = Task.FromResult(_droidDto);
            _repoMock
                .Setup(repo => repo.GetBySerialNumberAsync(_serialNumber, Droid.AssertExists))
                .Returns(getDroidResult);

            var getArmResult = Task.FromResult(armDtoList);
            _repoArmMock
                .Setup(repo => repo.GetDroidArmsAsync(_serialNumber))
                .Returns(getArmResult);

            // Act
            await droidHandler.HandleAsync(updateDroid).ConfigureAwait(false);

            // Assert
            _repoMock.Verify((r) => r.DoesNameAlreadyExistsAsync(updateDroid.Name), Times.Once);
            _repoMock.Verify((r) => r.UpdateAsync(
                It.Is<Droid>(
                    droid => droid.Name == updateDroid.Name 
                    && droid.Nickname == updateDroid.Nickname)), 
                Times.Once);
        }

        [Fact]
        public async Task HandleDeleteDroid_WithValidSerialNumber_DeletedCallsSuccessfully()
        {
            // Arrange
            var droidHandler = new DroidHandler(_repoMock.Object, _repoArmMock.Object, _quoteMock.Object, _encryptionMock.Object);
            var deleteDroid = new DeleteDroid(_serialNumber);
            IReadOnlyCollection<IArmDto> armDtoList = new List<ArmDto>
            {
                new ArmDto { DroidId = _serialNumber, ArmId = Guid.NewGuid() },
                new ArmDto { DroidId = _serialNumber, ArmId = Guid.NewGuid() }
            };

            var getDroidResult = Task.FromResult(_droidDto);
            _repoMock
                .Setup(repo => repo.GetBySerialNumberAsync(_serialNumber, Droid.AssertExists))
                .Returns(getDroidResult);

            var getArmResult = Task.FromResult(armDtoList);
            _repoArmMock
                .Setup(repo => repo.GetDroidArmsAsync(_serialNumber))
                .Returns(getArmResult);
            _repoMock
                .Setup(repo => repo.DeleteAsync(_serialNumber, It.IsAny<DeleteArmListAction>()))
                .Returns(Task.FromResult(true));

            // Act
            await droidHandler.HandleAsync(deleteDroid).ConfigureAwait(false);

            // Assert
            _repoMock.Verify((r) => r.DeleteAsync(_serialNumber, It.IsAny<DeleteArmListAction>()), Times.Once);
        }

        [Fact]
        public async Task HandleDeleteDroid_WithNonExistantSerialNumber_ThrowsDroidNotFoundException()
        {
            // Arrange
            var droidHandler = new DroidHandler(_repoMock.Object, _repoArmMock.Object, _quoteMock.Object, _encryptionMock.Object);
            var deleteDroid = new DeleteDroid(_serialNumber);
            var armIdList = new List<ArmId>
            {
                Guid.NewGuid()
            };
            _repoArmMock
                .Setup(repo => repo.DeleteAsync(armIdList, It.IsAny<object>()))
                .Returns(Task.FromResult(true));
            var actions = DeleteArmListAction.CreateNew(armIdList, _repoArmMock.Object.DeleteAsync);
            _repoMock
                .Setup(repo => repo.DeleteAsync(_serialNumber, actions))
                .Returns(Task.FromResult(false));

            // Act
            var exception = await Record.ExceptionAsync(
                () => droidHandler.HandleAsync(deleteDroid));

            // Assert
            exception.Should().BeOfType<DroidNotFoundException>();
        }

        [Fact]
        public async Task HandleCreateArm_WithValidDroidId_ArmCreatedSuccessfully()
        {
            // Arrange
            var droidHandler = new DroidHandler(_repoMock.Object, _repoArmMock.Object, _quoteMock.Object, _encryptionMock.Object);
            var createArm = new CreateArm(_serialNumber);
            
            var getDroidResult = Task.FromResult(_droidDto);
            _repoMock
                .Setup(repo => repo.GetBySerialNumberAsync(_serialNumber, Droid.AssertExists))
                .Returns(getDroidResult);

            var getArmResult = Task.FromResult(_armDtoList);
            _repoArmMock
                .Setup(repo => repo.GetDroidArmsAsync(_serialNumber))
                .Returns(getArmResult);
            _repoArmMock
                .Setup(repo => repo.InsertDroidArmAsync(_serialNumber, It.IsAny<Arm>(), It.IsAny<Action<IDroidValidationInfo>>()))
                .Returns(Task.CompletedTask);

            // Act
            await droidHandler.HandleAsync(createArm).ConfigureAwait(false);

            // Assert

            _repoArmMock.Verify((r) => r.InsertDroidArmAsync(_serialNumber, It.IsAny<Arm>(), It.IsAny<Action<IDroidValidationInfo>>()), Times.Once);
        }
    }
}
