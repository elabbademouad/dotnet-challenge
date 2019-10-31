using Cds.DroidManagement.Domain.DroidAggregate;
using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Infrastructure.DroidRepositories;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos;
using Dapper;
using FluentAssertions;
using Moq;
using Moq.Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cds.DroidManagement.Infrastructure.Tests.Unit
{
    public class SqlServerArmRepositoryTests
    {
        private readonly Mock<DbConnection> _connection;
        private readonly List<ArmDto> _armDtos;
        private readonly SqlServerArmRepository _repo;

        public SqlServerArmRepositoryTests()
        {
            _armDtos = new List<ArmDto>
            {
                new ArmDto { DroidId = Guid.Parse("341d9e5b-ae06-4c06-ba4f-7256317792cd"), ArmId = Guid.NewGuid() }
            };

            _connection = new Mock<DbConnection>();
            _repo = new SqlServerArmRepository(() => _connection.Object);
        }


        [Fact]
        public async Task GetDroidArms_WithValidSerialNumber_ReturnsArmListAsync()
        {
            // Arrange
            _connection
               .SetupDapperAsync(x => x.QueryAsync<ArmDto>(It.IsAny<string>(), null, null, null, null))
               .ReturnsAsync(_armDtos);

            // Act
            var result = await _repo.GetDroidArmsAsync(Guid.NewGuid());

            // Assert
            result.Should().HaveCount(_armDtos.Count);
        }

        [Fact]
        public async Task InsertDroidArm_WithValidArm_ShouldRaiseNoException()
        {
            // Arrange
            _connection
                .SetupDapperAsync(x => x.ExecuteAsync(It.IsAny<string>(), null, null, null, null))
                .ReturnsAsync(1);
            var droidId = Guid.NewGuid();
            var armDto = new ArmDto { DroidId = droidId, ArmId = Guid.NewGuid() };
            var arm = Arm.FromDto(armDto);

            // Act            
            var exception = await Record.ExceptionAsync(() => _repo.InsertDroidArmAsync(droidId, arm, _ => { }));

            // Assert
            exception.Should().BeNull();
        }
    }
}
