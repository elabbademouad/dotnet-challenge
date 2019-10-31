using Cds.DroidManagement.Domain.DroidAggregate;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos.Extensions;
using FluentAssertions;
using Xunit;

namespace Cds.DroidManagement.Infrastructure.Tests.Unit
{
    public class DroidExtensionTests
    {

        [Fact]
        public void DroidToDto_WithNullDroid_ReturnsNull()
        {
            // Arrange
            Droid droid = null;

            // Act
            var droidDto = droid.ToDto();

            // Assert
            droidDto.Should().BeNull();
        }
    }
}
