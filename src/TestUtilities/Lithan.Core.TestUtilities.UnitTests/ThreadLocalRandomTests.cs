using Xunit;
using AwesomeAssertions;
using Lithan.Core.TestUtilities;

namespace Lithan.Core.TestUtilities.UnitTests;

public class ThreadLocalRandomTests
{
    [Fact]
    public void Instance_ShouldReturnRandomInstance()
    {
        // Arrange

        // Act
        var instance = ThreadLocalRandom.Instance;

        // Assert
        instance.Should().NotBeNull();
        instance.Should().BeOfType<Random>();
    }

    [Fact]
    public void Instance_GivenSameThread_ShouldReturnSameInstance()
    {
        // Arrange

        // Act
        var first  = ThreadLocalRandom.Instance;
        var second = ThreadLocalRandom.Instance;

        // Assert
        first.Should().BeSameAs(second);
    }

    [Fact]
    public void NewRandom_ShouldReturnDistinctRandomInstances()
    {
        // Arrange

        // Act
        var first  = ThreadLocalRandom.NewRandom();
        var second = ThreadLocalRandom.NewRandom();

        // Assert
        first.Should().NotBeNull();
        second.Should().NotBeNull();
        first.Should().NotBeSameAs(second);
    }

    [Fact]
    public void Next_ShouldReturnNonNegativeValue()
    {
        // Arrange

        // Act
        var value = ThreadLocalRandom.Next();

        // Assert
        value.Should().BeGreaterThanOrEqualTo(0);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void Next_GivenMaxValue_ShouldReturnValueLessThanMaxValue(int maxValue)
    {
        // Arrange

        // Act
        var value = ThreadLocalRandom.Next(maxValue);

        // Assert
        value.Should().BeGreaterThanOrEqualTo(0);
        value.Should().BeLessThan(maxValue);
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(10, 20)]
    [InlineData(100, 200)]
    public void Next_GivenMinAndMaxValue_ShouldReturnValueWithinRange(int minValue, int maxValue)
    {
        // Arrange

        // Act
        var value = ThreadLocalRandom.Next(minValue, maxValue);

        // Assert
        value.Should().BeGreaterThanOrEqualTo(minValue);
        value.Should().BeLessThan(maxValue);
    }

    [Fact]
    public void NextDouble_ShouldReturnValueBetweenZeroAndOne()
    {
        // Arrange

        // Act
        var value = ThreadLocalRandom.NextDouble();

        // Assert
        value.Should().BeGreaterThanOrEqualTo(0d);
        value.Should().BeLessThan(1d);
    }

    [Fact]
    public void NextBytes_ShouldPopulateBuffer()
    {
        // Arrange
        var buffer = new byte[16];

        // Act
        ThreadLocalRandom.NextBytes(buffer);

        // Assert
        buffer.Should().NotBeEquivalentTo(new byte[16]);
    }
}
