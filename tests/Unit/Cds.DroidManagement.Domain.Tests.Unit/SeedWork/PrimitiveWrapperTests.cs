using Cds.DroidManagement.Domain.SeedWork;
using FluentAssertions;
using Xunit;

namespace Cds.DroidManagement.Domain.Tests.Unit.SeedWork
{
    public class PrimitiveWrapperTests
    {
        class WrapperTest : PrimitiveWrapper<int>
        {
            public WrapperTest(int value) : base(value)
            {
            }
        }

        [Fact]
        public void TwoPrimitiveWrapper_WithSameValues_HaveSameHashCode()
        {
            // Arrange
            var first = new WrapperTest(1);
            var second = new WrapperTest(1);

            // Act
            var firstHashCode = first.GetHashCode();
            var secondHashCode = second.GetHashCode();

            // Assert
            firstHashCode.Should().Be(secondHashCode);
        }

        [Fact]
        public void PrimitiveWrapper_CompareToNotNullPrimitiveWrapper_ShouldNotBeEqual()
        {
            // Arrange
            WrapperTest first = new WrapperTest(1);
            WrapperTest second = null;

            // Act
            var areEqual = first.Equals(second);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void PrimitiveWrapper_CompareToSamePrimitiveWrapper_ShouldBeEqual()
        {
            // Arrange
            var first = new WrapperTest(1);
            var second = first;

            // Act
            var areEqual = first.Equals(second);

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void PrimitiveWrapper_CompareToNotNulObject_ShouldBeNotEqual()
        {
            // Arrange
            var first = new WrapperTest(1);
            object second = (WrapperTest)null;

            // Act
            var areEqual = first.Equals(second);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void PrimitiveWrapper_CompareToSameObject_ShouldBeEqual()
        {
            // Arrange
            var first = new WrapperTest(1);
            object second = first;

            // Act
            var areEqual = first.Equals(second);

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void PrimitiveWrapper_CompareToOtherTypeObject_ShouldBeNotEqual()
        {
            // Arrange
            var first = new WrapperTest(1);
            object second = "toto";

            // Act
            var areEqual = first.Equals(second);

            // Assert
            areEqual.Should().BeFalse();
        }
    }
}
