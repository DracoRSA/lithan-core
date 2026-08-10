using Xunit;
using Xunit.Sdk;
using AwesomeAssertions;
using Lithan.Core.TestUtilities.UnitTests.Fakes;

namespace Lithan.Core.TestUtilities.UnitTests;

public class MethodTestHelperTests
{
    [Theory]
    [InlineData(null, "fakeComplex", "methodName")]
    [InlineData("TestMethod2", null, "parameterName")]
    public void ValidateArgumentNullExceptionIsThrownIfParameterIsNull_GivenParameterNull_ShouldThrowArgumentNullException(string? methodName,
                                                                                                                           string? parameterName,
                                                                                                                           string expectedName)
    {
        // Arrange

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => MethodTestHelper.ValidateArgumentNullExceptionIsThrownIfParameterIsNull<FakeTestClass>(methodName!, parameterName!));

        // Assert
        exception.ParamName.Should().Be(expectedName);
    }

    [Theory]
    [InlineData("TestMethod99", "fakeComplex")]
    [InlineData("TestMethod100", "fakeComplex")]
    public void ValidateArgumentNullExceptionIsThrownIfParameterIsNull_GivenParameterDoesNotExist_ShouldAssertFailedException(string methodName, string parameterName)
    {
        // Arrange
        var methodDoesNotExistMessage = $"Method [{methodName}] does not exists on";

        // Act
        var exception = Assert.Throws<FailException>(() => MethodTestHelper.ValidateArgumentNullExceptionIsThrownIfParameterIsNull<FakeTestClass>(methodName!, parameterName!));

        // Assert
        exception.Message.Should().Contain(methodDoesNotExistMessage);
    }

    [Theory]
    [InlineData("TestMethod2", "fakeComplex")]
    public void ValidateArgumentNullExceptionIsThrownIfParameterIsNull_GivenNullParameter_ShouldSucceedTest(string methodName, string parameterName)
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => MethodTestHelper.ValidateArgumentNullExceptionIsThrownIfParameterIsNull<FakeTestClass>(methodName, parameterName));

        // Assert
        exception.Should().BeNull();
    }

    [Theory]
    [InlineData("TestMethod2", "userName")]
    public void ValidateArgumentNullExceptionIsThrownIfParameterIsNull_GivenNotNullParameter_ShouldFailTest(string methodName, string parameterName)
    {
        // Arrange

        // Act
        var exception = Assert.Throws<FailException>(() => MethodTestHelper.ValidateArgumentNullExceptionIsThrownIfParameterIsNull<FakeTestClass>(methodName, parameterName));

        // Assert
        exception.Message.Should().Contain($"Argument Null Exception not throw for Method [{methodName}] Parameter [{parameterName}]");
    }

    [Theory]
    [InlineData(null, "fakeComplex", "methodName")]
    [InlineData("TestMethod2", null, "parameterName")]
    public void ValidateExceptionIsThrownIfParameterIsNull_GivenParameterNull_ShouldThrowArgumentNullException(string? methodName,
                                                                                                               string? parameterName,
                                                                                                               string expectedName)
    {
        // Arrange

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => MethodTestHelper.ValidateExceptionIsThrownIfParameterIsNull<FakeTestClass, ArgumentNullException>(methodName!, parameterName!));

        // Assert
        exception.ParamName.Should().Be(expectedName);
    }

    [Theory]
    [InlineData("TestMethod2", "fakeComplex")]
    public void ValidateExceptionIsThrownIfParameterIsNull_GivenNullParameter_ShouldSucceedTest(string methodName, string parameterName)
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => MethodTestHelper.ValidateExceptionIsThrownIfParameterIsNull<FakeTestClass, ArgumentNullException>(methodName, parameterName));

        // Assert
        exception.Should().BeNull();
    }

    [Theory]
    [InlineData("TestMethod2", "userName")]
    public void ValidateExceptionIsThrownIfParameterIsNull_GivenNotNullParameter_ShouldFailTest(string methodName, string parameterName)
    {
        // Arrange

        // Act
        var exception = Assert.Throws<FailException>(() => MethodTestHelper.ValidateExceptionIsThrownIfParameterIsNull<FakeTestClass, ArgumentNullException>(methodName, parameterName));

        // Assert
        exception.Message.Should().Contain("Exception not throw for Method");
    }

    [Theory]
    [InlineData("FakeTestMethod", "someParameter")]
    public void ValidateArgumentNullExceptionIsThrownIfParameterIsNull_GivenOverloadedMethod_ShouldResolveByParameterName(string methodName, string parameterName)
    {
        // Arrange

        // Act
        var exception = Assert.Throws<FailException>(() => MethodTestHelper.ValidateArgumentNullExceptionIsThrownIfParameterIsNull<FakeTestClass2>(methodName, parameterName));

        // Assert
        exception.Message.Should().Contain($"Argument Null Exception not throw for Method [{methodName}] Parameter [{parameterName}]");
    }

    [Theory]
    [InlineData(null, "fakeComplex", "methodName")]
    [InlineData("TestMethod3Async", null, "parameterName")]
    public async Task ValidateArgumentNullExceptionIsThrownIfParameterIsNullAsync_GivenParameterNull_ShouldThrowArgumentNullException(string? methodName,
                                                                                                                                      string? parameterName,
                                                                                                                                      string expectedName)
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => MethodTestHelper.ValidateArgumentNullExceptionIsThrownIfParameterIsNullAsync<FakeTestClass>(methodName!, parameterName!));

        // Assert
        exception.ParamName.Should().Be(expectedName);
    }

    [Theory]
    [InlineData("TestMethod3Async", "fakeComplex")]
    public async Task ValidateArgumentNullExceptionIsThrownIfParameterIsNullAsync_GivenNullParameter_ShouldSucceedTest(string methodName, string parameterName)
    {
        // Arrange

        // Act
        var exception = await Record.ExceptionAsync(() => MethodTestHelper.ValidateArgumentNullExceptionIsThrownIfParameterIsNullAsync<FakeTestClass>(methodName, parameterName));

        // Assert
        exception.Should().BeNull();
    }

    [Theory]
    [InlineData("TestMethod3Async", "userName")]
    public async Task ValidateArgumentNullExceptionIsThrownIfParameterIsNullAsync_GivenNotNullParameter_ShouldFailTest(string methodName, string parameterName)
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<FailException>(() => MethodTestHelper.ValidateArgumentNullExceptionIsThrownIfParameterIsNullAsync<FakeTestClass>(methodName, parameterName));

        // Assert
        exception.Message.Should().Contain($"Argument Null Exception not throw for Method [{methodName}] Parameter [{parameterName}]");
    }

    [Theory]
    [InlineData(null, "fakeComplex", "methodName")]
    [InlineData("TestMethod3Async", null, "parameterName")]
    public async Task ValidateExceptionIsThrownIfParameterIsNullAsync_GivenParameterNull_ShouldThrowArgumentNullException(string? methodName,
                                                                                                                          string? parameterName,
                                                                                                                          string expectedName)
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => MethodTestHelper.ValidateExceptionIsThrownIfParameterIsNullAsync<FakeTestClass, ArgumentNullException>(methodName!, parameterName!));

        // Assert
        exception.ParamName.Should().Be(expectedName);
    }

    [Theory]
    [InlineData("TestMethod3Async", "fakeComplex")]
    public async Task ValidateExceptionIsThrownIfParameterIsNullAsync_GivenNullParameter_ShouldSucceedTest(string methodName, string parameterName)
    {
        // Arrange

        // Act
        var exception = await Record.ExceptionAsync(() => MethodTestHelper.ValidateExceptionIsThrownIfParameterIsNullAsync<FakeTestClass, ArgumentNullException>(methodName, parameterName));

        // Assert
        exception.Should().BeNull();
    }

    [Theory]
    [InlineData("TestMethod3Async", "userName")]
    public async Task ValidateExceptionIsThrownIfParameterIsNullAsync_GivenNotNullParameter_ShouldFailTest(string methodName, string parameterName)
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<FailException>(() => MethodTestHelper.ValidateExceptionIsThrownIfParameterIsNullAsync<FakeTestClass, ArgumentNullException>(methodName, parameterName));

        // Assert
        exception.Message.Should().Contain("Exception not throw for Method");
    }
}
