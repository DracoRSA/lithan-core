using Xunit;
using Xunit.Sdk;
using AwesomeAssertions;
using Lithan.Core.TestUtilities.XUnit;

namespace Lithan.Core.TestUtilities.UnitTests;

public class XUnitAssertExtensionsTests
{
    [Fact]
    public void DoesNotThrow_GivenActionThatDoesNotThrow_ShouldPassTest()
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => XUnitAssertExtensions.DoesNotThrow(null!, () => { }));

        // Assert
        exception.Should().BeNull();
    }

    [Fact]
    public void DoesNotThrow_GivenActionThatThrows_ShouldFailTest()
    {
        // Arrange

        // Act
        var exception = Assert.Throws<FailException>(() => XUnitAssertExtensions.DoesNotThrow(null!, () => throw new InvalidOperationException("boom")));

        // Assert
        exception.Message.Should().Contain("Expected not to throw Exception, but Exception was thrown");
        exception.Message.Should().Contain("boom");
    }

    [Fact]
    public void DoesNotThrow_GivenCustomErrorMessage_ShouldFailWithCustomMessage()
    {
        // Arrange
        var errorMessage = "Custom failure message";

        // Act
        var exception = Assert.Throws<FailException>(() => XUnitAssertExtensions.DoesNotThrow(null!, () => throw new Exception(), errorMessage));

        // Assert
        exception.Message.Should().Be(errorMessage);
    }

    [Fact]
    public void DoesNotThrow_GivenGenericAndActionThatDoesNotThrow_ShouldPassTest()
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => XUnitAssertExtensions.DoesNotThrow<ArgumentNullException>(null!, () => { }));

        // Assert
        exception.Should().BeNull();
    }

    [Fact]
    public void DoesNotThrow_GivenGenericAndMatchingException_ShouldFailTest()
    {
        // Arrange

        // Act
        var exception = Assert.Throws<FailException>(() => XUnitAssertExtensions.DoesNotThrow<ArgumentNullException>(null!, () => throw new ArgumentNullException("param")));

        // Assert
        exception.Message.Should().Contain($"Expected not to throw {typeof(ArgumentNullException)} Exception");
    }

    [Fact]
    public void DoesNotThrow_GivenGenericAndDifferentException_ShouldPropagateException()
    {
        // Arrange

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => XUnitAssertExtensions.DoesNotThrow<ArgumentNullException>(null!, () => throw new InvalidOperationException("other")));

        // Assert
        exception.Message.Should().Be("other");
    }

    [Fact]
    public void DoesNotThrow_GivenGenericAndCustomErrorMessage_ShouldFailWithCustomMessage()
    {
        // Arrange
        var errorMessage = "Typed custom failure";

        // Act
        var exception = Assert.Throws<FailException>(() => XUnitAssertExtensions.DoesNotThrow<ArgumentException>(null!, () => throw new ArgumentException("x"), errorMessage));

        // Assert
        exception.Message.Should().Be(errorMessage);
    }
}
