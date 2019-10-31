using Cds.DroidManagement.Domain.DroidAggregate;
using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using Cds.DroidManagement.Infrastructure.DroidRepositories;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cds.DroidManagement.Infrastructure.Tests.Unit
{
    public class InMemoryArmRepositoryTests
    {
        private readonly InMemoryArmRepository _repo = new InMemoryArmRepository();
        private readonly Guid _existingDroidId = Guid.Parse("341d9e5b-ae06-4c06-ba4f-7256317792cd");
        private readonly Guid _existingArmId = Guid.Parse("8ef70fa1-5822-433d-9cb6-e39595eec42a");

        [Fact]
        public async Task GetArmForDroid_WithserialNumber_SelectArm()
        {
            // Act
            var armList = await _repo.GetDroidArmsAsync(_existingDroidId);

            // Assert
            armList.Should().HaveCount(1);
        }

        [Fact]
        public async Task DeleteArmForDroid_WithValidSerialNumber_DroidDeletedSuccessfully()
        {
            // Act
            await _repo.DeleteAsync(new List<ArmId> { _existingArmId }, null);
            var armList = await _repo.GetDroidArmsAsync(_existingArmId);

            //Assert            
            armList.Should().HaveCount(0);
        }

        [Fact]
        public async Task AddArm_WithValidDroidId_ArmCreatedSuccessfully()
        {
            // Arrange
            var armId = Guid.Parse("eec8187e-2be7-4f9b-864b-84f66b2eab72");
            var armDto = new ArmDto
            {
                ArmId = armId,
                DroidId = _existingDroidId // Useless but required with DromDto
            };
            var arm = Arm.FromDto(armDto);

            // Act
            await _repo.InsertDroidArmAsync(_existingDroidId, arm, _ => { });

            // Assert
            var armList = await _repo.GetDroidArmsAsync(_existingDroidId);
            armList.Should().HaveCount(2);
        }
    }
}
