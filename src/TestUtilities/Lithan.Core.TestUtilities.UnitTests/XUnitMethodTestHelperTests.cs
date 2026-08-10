using Xunit;
using Xunit.Sdk;
using AwesomeAssertions;
using Lithan.Core.TestUtilities.XUnit;
using Lithan.Core.TestUtilities.UnitTests.Fakes;

namespace Lithan.Core.TestUtilities.UnitTests;

public class XUnitMethodTestHelperTests
{
    [Theory]
    [InlineData(null, "fakeComplex", "methodName")]
    [InlineData("TestMethod2", null, "parameterName")]
    public void ValidateArgumentNullExceptionIfParameterIsNull_GivenParameterNull_ShouldThrowArgumentNullException(string? methodName,
                                                                                                                   string? parameterName,
                                                                                                                   string expectedName)
    {
        // Arrange

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => XUnitMethodTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<FakeTestClass>(methodName!, parameterName!));

        // Assert
        exception.ParamName.Should().Be(expectedName);
    }

    [Theory]
    [InlineData("TestMethod99", "fakeComplex")]
    [InlineData("TestMethod100", "fakeComplex")]
    public void ValidateArgumentNullExceptionIfParameterIsNull_GivenMethodDoesNotExist_ShouldFailTest(string methodName, string parameterName)
    {
        // Arrange
        var methodDoesNotExistMessage = $"Method [{methodName}] does not exists on";

        // Act
        var exception = Assert.Throws<FailException>(() => XUnitMethodTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<FakeTestClass>(methodName, parameterName));

        // Assert
        exception.Message.Should().Contain(methodDoesNotExistMessage);
    }

    [Theory]
    [InlineData("TestMethod2", "fakeComplex")]
    public void ValidateArgumentNullExceptionIfParameterIsNull_GivenNullParameter_ShouldPassTest(string methodName, string parameterName)
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => XUnitMethodTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<FakeTestClass>(methodName, parameterName));

        // Assert
        exception.Should().BeNull();
    }

    [Theory]
    [InlineData("TestMethod2", "userName")]
    public void ValidateArgumentNullExceptionIfParameterIsNull_GivenParameterThatDoesNotThrow_ShouldFailTest(string methodName, string parameterName)
    {
        // Arrange

        // Act
        var exception = Assert.Throws<FailException>(() => XUnitMethodTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<FakeTestClass>(methodName, parameterName));

        // Assert
        exception.Message.Should().Contain($"Argument Null Exception not throw for Method [{methodName}] Parameter [{parameterName}]");
    }

    [Theory]
    [InlineData("FakeTestMethod", "someParameter")]
    public void ValidateArgumentNullExceptionIfParameterIsNull_GivenOverloadedMethod_ShouldResolveByParameterName(string methodName, string parameterName)
    {
        // Arrange

        // Act
        var exception = Assert.Throws<FailException>(() => XUnitMethodTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<FakeTestClass2>(methodName, parameterName));

        // Assert
        exception.Message.Should().Contain($"Argument Null Exception not throw for Method [{methodName}] Parameter [{parameterName}]");
    }

    [Theory]
    [InlineData(null, "fakeComplex", "methodName")]
    [InlineData("TestMethod3Async", null, "parameterName")]
    public async Task ValidateArgumentNullExceptionIfParameterIsNullAsync_GivenParameterNull_ShouldThrowArgumentNullException(string? methodName,
                                                                                                                              string? parameterName,
                                                                                                                              string expectedName)
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => XUnitMethodTestHelper.ValidateArgumentNullExceptionIfParameterIsNullAsync<FakeTestClass>(methodName!, parameterName!));

        // Assert
        exception.ParamName.Should().Be(expectedName);
    }

    [Theory]
    [InlineData("TestMethod3Async", "fakeComplex")]
    public async Task ValidateArgumentNullExceptionIfParameterIsNullAsync_GivenNullParameter_ShouldPassTest(string methodName, string parameterName)
    {
        // Arrange

        // Act
        var exception = await Record.ExceptionAsync(() => XUnitMethodTestHelper.ValidateArgumentNullExceptionIfParameterIsNullAsync<FakeTestClass>(methodName, parameterName));

        // Assert
        exception.Should().BeNull();
    }

    [Theory]
    [InlineData("TestMethod3Async", "userName")]
    public async Task ValidateArgumentNullExceptionIfParameterIsNullAsync_GivenParameterThatDoesNotThrow_ShouldFailTest(string methodName, string parameterName)
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<FailException>(() => XUnitMethodTestHelper.ValidateArgumentNullExceptionIfParameterIsNullAsync<FakeTestClass>(methodName, parameterName));

        // Assert
        exception.Message.Should().Contain($"Argument Null Exception not throw for Method [{methodName}] Parameter [{parameterName}]");
    }
}
