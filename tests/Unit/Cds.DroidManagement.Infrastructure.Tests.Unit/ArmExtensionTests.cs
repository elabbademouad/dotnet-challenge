using Cds.DroidManagement.Domain.DroidAggregate;
using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using FluentAssertions;
using System;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos.Extensions;
using Xunit;

namespace Cds.DroidManagement.Infrastructure.Tests.Unit
{
    public class ArmExtensionTests
    {

        [Fact]
        public void ArmToDto_WithNullArm_ReturnsNull()
        {
            // Arrange
            Arm arm = null;
            DroidId droidId = Guid.NewGuid();

            // Act
            var armDto = arm.ToDto(droidId);

            // Assert
            armDto.Should().BeNull();
        }
    }
}
