using Xunit;
using AwesomeAssertions;
using Lithan.Core.Models;
using Lithan.Core.TestUtilities.XUnit;

namespace Lithan.Core.UnitTests.Models;

public class LithanErrorTests
{
    [Fact]
    public void Constructor()
    {
        // Arrange

        // Act
        var response = new LithanError(1, "Test Error");

        // Assert
        response.Should().NotBeNull();
    }

    [Theory]
    [InlineData("message")]
    public void Constructor_GivenNullParameterValue_ShouldThrowArgumentNullException(string parameterName)
    {
        // Arrange

        // Act
        XUnitConstructorTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<LithanError>(parameterName);

        // Assert
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Constructor_GivenInvalidErrorCode_ShouldThrowArgumentOutOfRangeException(int invalidErrorCode)
    {
        // Arrange

        // Act
        XUnitConstructorTestHelper.ValidateExceptionIsThrownIfParameterIsNull<LithanError, ArgumentOutOfRangeException>("errorCode", false, ("errorCode", invalidErrorCode));

        // Assert
    }

    [Theory]
    [InlineData("errorCode", "ErrorCode")]
    [InlineData("message", "Message")]
    [InlineData("exception", "Exception")]
    public void Constructor_GivenParameterValue_ShouldSetPropertyValue(string parameterName, string propertyName)
    {
        // Arrange

        // Act
        XUnitConstructorTestHelper.ValidatePropertySetWithParameter<LithanError>(parameterName, propertyName);

        // Assert
    }

    [Fact]
    public void ToString_ShouldReturnExpectedValue()
    {
        // Arrange
        var errorCode    = 999;
        var message      = "Test Message";
        var exception    = new Exception("Test Exception");
        var expectedData = $"Error: {errorCode} - {message}{$"\n{exception}"}";
        var dxcError     = new LithanError(999, "Test Message", new Exception("Test Exception"));

        // Act
        var returnedData = dxcError.ToString();

        // Assert
        returnedData.Should().Be(expectedData);
    }
}