using Xunit;
using AwesomeAssertions;
using Lithan.Core.Models;
using Lithan.Core.Result;

namespace Lithan.Core.UnitTests.Result;

public class LithanResultNonGenericTests
{
    [Fact]
    public void Constructor_GivenSuccess_ShouldSetPropertiesToExpectedValues()
    {
        // Arrange

        // Act
        var result = new LithanResult();

        // Assert
        result.IsError.Should().BeFalse();
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Constructor_GivenError_ShouldSetPropertiesToExpectedValues()
    {
        // Arrange
        var resultError = new LithanError(1, "Test Error Message");

        // Act
        var result = new LithanResult(resultError);

        // Assert
        result.IsError.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(resultError);
    }

    [Fact]
    public void Success_ShouldSetPropertiesToExpectedValues()
    {
        // Arrange

        // Act
        var result = LithanResult.Success();

        // Assert
        result.IsError.Should().BeFalse();
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failure_ShouldSetPropertiesToExpectedValues()
    {
        // Arrange
        var resultError = new LithanError(1, "Test Error Message");

        // Act
        var result = LithanResult.Failure(resultError);

        // Assert
        result.IsError.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(resultError);
    }

    [Fact]
    public void ImplicitOperator_GivenError_ShouldCreateFailureResult()
    {
        // Arrange
        var resultError = new LithanError(2, "Implicit Error");

        // Act
        LithanResult result = resultError;

        // Assert
        result.IsError.Should().BeTrue();
        result.Error.Should().Be(resultError);
    }

    [Fact]
    public void Match_GivenSuccess_ShouldExecuteSuccessPath()
    {
        // Arrange
        var result        = LithanResult.Success();
        var successCalled = false;

        // Act
        result.Match(success: () => successCalled = true,
                     failure: _ => Assert.Fail("Failure should not be called"));

        // Assert
        successCalled.Should().BeTrue();
    }

    [Fact]
    public void Match_GivenFailure_ShouldExecuteFailurePath()
    {
        // Arrange
        var result        = LithanResult.Failure(new LithanError(9, "fail"));
        var failureCalled = false;

        // Act
        result.Match(success: () => Assert.Fail("Success should not be called"),
                     failure: _ => failureCalled = true);

        // Assert
        failureCalled.Should().BeTrue();
    }

    [Fact]
    public void Match_GivenSuccessAndSuccessOnlyOverload_ShouldExecuteSuccessPath()
    {
        // Arrange
        var result        = LithanResult.Success();
        var successCalled = false;

        // Act
        result.Match(() => successCalled = true);

        // Assert
        successCalled.Should().BeTrue();
    }

    [Fact]
    public void Match_GivenFailureAndSuccessOnlyOverload_ShouldNotExecuteSuccessPath()
    {
        // Arrange
        var result        = LithanResult.Failure(new LithanError(9, "fail"));
        var successCalled = false;

        // Act
        result.Match(() => successCalled = true);

        // Assert
        successCalled.Should().BeFalse();
    }

    [Fact]
    public void Match_GivenSuccessAndTypedOverload_ShouldReturnSuccessValue()
    {
        // Arrange
        var result = LithanResult.Success();

        // Act
        var matched = result.Match(success: () => "ok",
                                   failure: _ => "error");

        // Assert
        matched.Should().Be("ok");
    }

    [Fact]
    public void Match_GivenFailureAndTypedOverload_ShouldReturnFailureValue()
    {
        // Arrange
        var result = LithanResult.Failure(new LithanError(9, "fail"));

        // Act
        var matched = result.Match(success: () => "ok",
                                   failure: _ => "error");

        // Assert
        matched.Should().Be("error");
    }

    [Fact]
    public void Match_GivenSuccessAndTypedSuccessOnlyOverload_ShouldReturnSuccessValue()
    {
        // Arrange
        var result = LithanResult.Success();

        // Act
        var matched = result.Match(() => "ok");

        // Assert
        matched.Should().Be("ok");
    }

    [Fact]
    public void Match_GivenFailureAndTypedSuccessOnlyOverload_ShouldReturnDefault()
    {
        // Arrange
        var result = LithanResult.Failure(new LithanError(9, "fail"));

        // Act
        var matched = result.Match(() => "ok");

        // Assert
        matched.Should().BeNull();
    }

    [Fact]
    public async Task MatchAsync_GivenSuccess_ShouldExecuteSuccessPath()
    {
        // Arrange
        var result        = LithanResult.Success();
        var successCalled = false;

        // Act
        await result.MatchAsync(success: async () =>
                                         {
                                             successCalled = true;
                                             await Task.CompletedTask;
                                         },
                                failure: async _ =>
                                         {
                                             Assert.Fail("Failure should not be called");
                                             await Task.CompletedTask;
                                         });

        // Assert
        successCalled.Should().BeTrue();
    }

    [Fact]
    public async Task MatchAsync_GivenFailure_ShouldExecuteFailurePath()
    {
        // Arrange
        var result        = LithanResult.Failure(new LithanError(9, "fail"));
        var failureCalled = false;

        // Act
        await result.MatchAsync(success: async () =>
                                         {
                                             Assert.Fail("Success should not be called");
                                             await Task.CompletedTask;
                                         },
                                failure: async _ =>
                                         {
                                             failureCalled = true;
                                             await Task.CompletedTask;
                                         });

        // Assert
        failureCalled.Should().BeTrue();
    }

    [Fact]
    public async Task MatchAsync_GivenSuccessAndSuccessOnlyOverload_ShouldExecuteSuccessPath()
    {
        // Arrange
        var result        = LithanResult.Success();
        var successCalled = false;

        // Act
        await result.MatchAsync(async () =>
                                {
                                    successCalled = true;
                                    await Task.CompletedTask;
                                });

        // Assert
        successCalled.Should().BeTrue();
    }

    [Fact]
    public async Task MatchAsync_GivenFailureAndSuccessOnlyOverload_ShouldNotExecuteSuccessPath()
    {
        // Arrange
        var result        = LithanResult.Failure(new LithanError(9, "fail"));
        var successCalled = false;

        // Act
        await result.MatchAsync(async () =>
                                {
                                    successCalled = true;
                                    await Task.CompletedTask;
                                });

        // Assert
        successCalled.Should().BeFalse();
    }

    [Fact]
    public async Task MatchAsync_GivenSuccessAndTypedOverload_ShouldReturnSuccessValue()
    {
        // Arrange
        var result = LithanResult.Success();

        // Act
        var matched = await result.MatchAsync(success: async () => await Task.FromResult("ok"),
                                              failure: async _ => await Task.FromResult("error"));

        // Assert
        matched.Should().Be("ok");
    }

    [Fact]
    public async Task MatchAsync_GivenFailureAndTypedOverload_ShouldReturnFailureValue()
    {
        // Arrange
        var result = LithanResult.Failure(new LithanError(9, "fail"));

        // Act
        var matched = await result.MatchAsync(success: async () => await Task.FromResult("ok"),
                                              failure: async _ => await Task.FromResult("error"));

        // Assert
        matched.Should().Be("error");
    }

    [Fact]
    public async Task MatchAsync_GivenSuccessAndTypedSuccessOnlyOverload_ShouldReturnSuccessValue()
    {
        // Arrange
        var result = LithanResult.Success();

        // Act
        var matched = await result.MatchAsync(async () => await Task.FromResult("ok"));

        // Assert
        matched.Should().Be("ok");
    }

    [Fact]
    public async Task MatchAsync_GivenFailureAndTypedSuccessOnlyOverload_ShouldReturnDefault()
    {
        // Arrange
        var result = LithanResult.Failure(new LithanError(9, "fail"));

        // Act
        var matched = await result.MatchAsync(async () => await Task.FromResult("ok"));

        // Assert
        matched.Should().BeNull();
    }
}
