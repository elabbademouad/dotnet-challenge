using Cds.DroidManagement.Domain.DroidAggregate;
using FluentAssertions;
using System;
using Xunit;

namespace Cds.DroidManagement.Domain.Tests.Unit
{
    public class ArmTests
    {
        [Fact]
        public void CreateArm_WithoutParameter_CreatedArmSucessfully()
        {
            // Act
            var arm = Arm.CreateNew();

            // Assert
            arm.Should().NotBeNull();
            arm.SerialNumber.Should().NotBe(default(Guid));
        }

        [Fact]
        public void FromDto_WithNullArm_ReturnsNull()
        {
            // Act
            var arm = Arm.FromDto(null);

            // Assert
            arm.Should().BeNull();
        }
    }
}
