using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Cds.DroidManagement.Domain.DroidAggregate;
using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Domain.DroidAggregate.RepositoryAction;
using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using Cds.DroidManagement.Infrastructure.DroidRepositories;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos;
using Dapper;
using FluentAssertions;
using Moq;
using Moq.Dapper;
using Xunit;

namespace Cds.DroidManagement.Infrastructure.Tests.Unit
{
    public class SqlServerDroidRepositoryTests
    {
        private readonly List<DroidDto> _droidDtos;
        private readonly List<ArmDto> _armDtos;
        private readonly Mock<DbConnection> _connection;
        private readonly SqlServerDroidRepository _repo;
        private readonly Mock<IArmRepository> _repoArm;

        public SqlServerDroidRepositoryTests()
        {
            _droidDtos = new List<DroidDto>
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

            _armDtos = new List<ArmDto>
            {
                new ArmDto { DroidId = _droidDtos[0].DroidId, ArmId = Guid.NewGuid() }
            };

            _connection = new Mock<DbConnection>();
            _repo = new SqlServerDroidRepository(() => _connection.Object);
            _repoArm = new Mock<IArmRepository>();
        }

        [Fact]
        public async Task GetAllPaged_WithNotAllDroidsInPage_ReturnsDroidsWithNextPage()
        {
            // Arrange
            _connection
                .SetupDapperAsync(x => x.QueryAsync<DroidDto>(It.IsAny<string>(), null, null, null, null))
                .ReturnsAsync(_droidDtos);

            // Act
            var (droidList, hasNext) = await _repo.GetAllPagedAsync(0, 1).ConfigureAwait(false);

            // Assert
            droidList.Should().HaveCount(1);
            hasNext.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllPaged_WithAllDroidsInPage_ReturnsDroidsWithNoNextPage()
        {
            // Arrange
            _connection
                .SetupDapperAsync(x => x.QueryAsync<DroidDto>(It.IsAny<string>(), null, null, null, null))
                .ReturnsAsync(_droidDtos);

            // Act
            var (droidList, hasNext) = await _repo.GetAllPagedAsync(0, 2).ConfigureAwait(false);

            // Assert
            droidList.Should().HaveCount(2);
            hasNext.Should().BeFalse();
        }

        [Fact]
        public async Task GetPagedDroid_WithPagination_DroidListWithNextPageAsync()
        {
            // Arrange
            _connection
                .SetupDapperAsync(x => x.QueryAsync<DroidDto>(It.IsAny<string>(), null, null, null, null))
                .ReturnsAsync(_droidDtos);

            // Act
            var (result, hasNext) = await _repo.GetAllPagedAsync(1, 1).ConfigureAwait(false);

            // Assert
            result.Should().HaveCount(1);
            hasNext.Should().BeTrue();
        }

        [Fact]
        public async Task GetBySerialNumber_WithValidSerialNumber_ReturnsDroidSelectedAsync()
        {
            // Arrange
            _connection
                .SetupDapperAsync(x => x.QueryAsync<DroidDto>(It.IsAny<string>(), null, null, null, null))
                .ReturnsAsync(_droidDtos);
            var droidDto = _droidDtos[0];

            // Act
            var result = await _repo.GetBySerialNumberAsync(droidDto.DroidId, Droid.AssertExists);

            // Assert
            result.DroidId.Should().Be(droidDto.DroidId);
            result.Name.Should().Be(droidDto.Name);
            result.Nickname.Should().Be(droidDto.Nickname);
        }

        [Fact]
        public async Task UpdateDroid_WithValidDroid_ShouldRaiseNoException()
        {
            // Arrange
            _connection
                .SetupDapperAsync(x => x.ExecuteAsync(It.IsAny<string>(), null, null, null, null))
                .ReturnsAsync(1);
            var droid = Droid.FromDto(_droidDtos[0]).WithArms(_armDtos);

            // Act            
            var exception = await Record.ExceptionAsync(() => _repo.UpdateAsync(droid));

            // Assert
            exception.Should().BeNull();
        }

        [Fact]
        public async Task InsertDroid_WithValidDroid_ShouldRaiseNoException()
        {
            // Arrange
            _connection
                .SetupDapperAsync(x => x.ExecuteAsync(It.IsAny<string>(), null, null, null, null))
                .ReturnsAsync(1);
            var droid = Droid.FromDto(_droidDtos[0]).WithArms(_armDtos);

            // Act            
            var exception = await Record.ExceptionAsync(() => _repo.InsertAsync(droid)).ConfigureAwait(false);

            // Assert
            exception.Should().BeNull();
        }

        [Fact]
        public async Task DeleteDroid_WithValidSerialNumber_ReturnsTrue()
        {
            // Arrange
            _connection
                .SetupDapperAsync(x => x.ExecuteAsync(It.IsAny<string>(), null, null, null, null))
                .ReturnsAsync(1);
            var armIdList = new List<ArmId>
            {
                Guid.NewGuid()
            };

            _repoArm.Setup(repo => repo.DeleteAsync(armIdList, It.IsAny<IDbConnection>())).ReturnsAsync(true);

            var actions = DeleteArmListAction.CreateNew(armIdList, _repoArm.Object.DeleteAsync);

            // Act
            var result = await _repo.DeleteAsync(Guid.NewGuid(), actions);

            // Assert
            _repoArm.Verify(repo => repo.DeleteAsync(armIdList, It.IsAny<IDbConnection>()), Times.Once);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DoesNameAlreadyExists_WithExistingName_ReturnsTrue()
        {
            // Arrange
            _connection
                .SetupDapperAsync(x => x.QueryAsync<bool>(It.IsAny<string>(), null, null, null, null))
                .ReturnsAsync(new List<bool> { true });

            // Act
            var result = await _repo.DoesNameAlreadyExistsAsync("Toto").ConfigureAwait(false);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteDroidTransaction_WithValidSerialNumber_ReturnsTrue()
        {
            // Arrange
            _connection
                .SetupDapperAsync(x => x.ExecuteAsync(It.IsAny<string>(), null, null, null, null))
                .ReturnsAsync(1);
            var armIdList = new List<ArmId>
            {
                Guid.NewGuid()
            };

            _repoArm.Setup(repo => repo.DeleteAsync(armIdList, It.IsAny<IDbConnection>())).ReturnsAsync(true);

            var actions = DeleteArmListAction.CreateNew(armIdList, _repoArm.Object.DeleteAsync);

            // Act
            var result = await _repo.DeleteAsync(Guid.NewGuid(), actions).ConfigureAwait(false);

            // Assert
            _repoArm.Verify(repo => repo.DeleteAsync(armIdList, It.IsAny<IDbConnection>()), Times.Once);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteDroidTransaction_WithDroidWithInvalidSerialNumber_ReturnsFalse()
        {
            // Arrange
            _connection
                .SetupDapperAsync(x => x.ExecuteAsync(It.IsAny<string>(), null, null, null, null))
                .ReturnsAsync(0);
            var armIdList = new List<ArmId>
            {
                Guid.NewGuid()
            };

            _repoArm.Setup(repo => repo.DeleteAsync(armIdList, It.IsAny<IDbConnection>())).ReturnsAsync(false);

            var actions = DeleteArmListAction.CreateNew(armIdList, _repoArm.Object.DeleteAsync);

            // Act
            var result = await _repo.DeleteAsync(Guid.NewGuid(), actions);

            // Assert
            _repoArm.Verify(repo => repo.DeleteAsync(armIdList, It.IsAny<IDbConnection>()), Times.Once);
            result.Should().BeFalse();
        }
    }
}
