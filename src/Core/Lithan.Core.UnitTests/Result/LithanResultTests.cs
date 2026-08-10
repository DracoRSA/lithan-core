using Xunit;
using AwesomeAssertions;
using Lithan.Core.Models;
using Lithan.Core.Result;

namespace Lithan.Core.UnitTests.Result;

public class LithanResultTests
{
    [Fact]
    public void Constructor()
    {
        // Arrange

        // Act
        var dxcResult = new LithanResult<bool>(true);

        // Assert
        dxcResult.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_GivenSuccess_ShouldSetPropertiesToExpectedValues()
    {
        // Arrange

        // Act
        var dxcResult = new LithanResult<bool>(true);

        // Assert
        dxcResult.IsError.Should().BeFalse();
        dxcResult.IsSuccess.Should().BeTrue();
        dxcResult.Value.Should().BeTrue();
        dxcResult.Error.Should().BeNull();
    }

    [Fact]
    public void Constructor_GivenError_ShouldSetPropertiesToExpectedValues()
    {
        // Arrange
        var resultError = new LithanError(1, "Test Error Message");

        // Act
        var dxcResult = new LithanResult<bool>(resultError);

        // Assert
        dxcResult.IsError.Should().BeTrue();
        dxcResult.IsSuccess.Should().BeFalse();
        dxcResult.Value.Should().BeFalse();
        dxcResult.Error.Should().Be(resultError);
    }

    [Fact]
    public void Success_ShouldSetPropertiesToExpectedValues()
    {
        // Arrange

        // Act
        var dxcResult = LithanResult<bool>.Success(true);

        // Assert
        dxcResult.IsError.Should().BeFalse();
        dxcResult.IsSuccess.Should().BeTrue();
        dxcResult.Value.Should().BeTrue();
        dxcResult.Error.Should().BeNull();
    }

    [Fact]
    public void Failure_ShouldSetPropertiesToExpectedValues()
    {
        // Arrange
        var resultError = new LithanError(1, "Test Error Message");

        // Act
        var dxcResult = LithanResult<bool>.Failure(resultError);

        // Assert
        dxcResult.IsError.Should().BeTrue();
        dxcResult.IsSuccess.Should().BeFalse();
        dxcResult.Value.Should().BeFalse();
        dxcResult.Error.Should().Be(resultError);
    }

    [Fact]
    public void ImplicitOperator_GivenValue_ShouldCreateSuccessResult()
    {
        // Arrange

        // Act
        LithanResult<string> result = "value";

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("value");
    }

    [Fact]
    public void ImplicitOperator_GivenError_ShouldCreateFailureResult()
    {
        // Arrange
        var resultError = new LithanError(3, "error");

        // Act
        LithanResult<string> result = resultError;

        // Assert
        result.IsError.Should().BeTrue();
        result.Error.Should().Be(resultError);
    }

    [Fact]
    public void Match_GivenSuccess_ShouldExecuteExpectedPath()
    {
        // Arrange
        var dxcResult = LithanResult<bool>.Success(true);

        // Act
        dxcResult.Match(success: Assert.True,
                        failure: _ => Assert.Fail("Failure should not be called"));

        // Assert
    }

    [Fact]
    public void Match_GivenFailure_ShouldExecuteExpectedPath()
    {
        // Arrange
        var dxcResult = LithanResult<bool>.Failure(new LithanError(999, "Test Error"));

        // Act
        dxcResult.Match(success: _ => Assert.Fail("Success should not be called"),
                        failure: _ => Assert.True(true));

        // Assert
    }

    [Fact]
    public void Match_GivenSuccessNull_ShouldExecuteExpectedPath()
    {
        // Arrange
        var dxcResult = LithanResult<string?>.Success(null);

        // Act
        dxcResult.Match(success: _ => Assert.Fail("Success should not be called"),
                        failure: _ => Assert.Fail("Failure should not be called"),
                        nullValue: () => Assert.True(true));

        // Assert
    }

    [Fact]
    public void Match_GivenSuccessAndSuccessOnlyOverload_ShouldExecuteSuccessPath()
    {
        // Arrange
        var result        = LithanResult<string>.Success("ok");
        var successCalled = false;

        // Act
        result.Match(_ => successCalled = true);

        // Assert
        successCalled.Should().BeTrue();
    }

    [Fact]
    public void Match_GivenFailureAndSuccessOnlyOverload_ShouldNotExecuteSuccessPath()
    {
        // Arrange
        var result        = LithanResult<string>.Failure(new LithanError(1, "fail"));
        var successCalled = false;

        // Act
        result.Match(_ => successCalled = true);

        // Assert
        successCalled.Should().BeFalse();
    }

    [Fact]
    public void Match_GivenSuccessAndTypedOverload_ShouldReturnSuccessValue()
    {
        // Arrange
        var result = LithanResult<int>.Success(5);

        // Act
        var matched = result.Match(success: value => value * 2,
                                   failure: _ => -1);

        // Assert
        matched.Should().Be(10);
    }

    [Fact]
    public void Match_GivenFailureAndTypedOverload_ShouldReturnFailureValue()
    {
        // Arrange
        var result = LithanResult<int>.Failure(new LithanError(1, "fail"));

        // Act
        var matched = result.Match(success: value => value * 2,
                                   failure: _ => -1);

        // Assert
        matched.Should().Be(-1);
    }

    [Fact]
    public void Match_GivenSuccessAndTypedSuccessOnlyOverload_ShouldReturnSuccessValue()
    {
        // Arrange
        var result = LithanResult<int>.Success(5);

        // Act
        var matched = result.Match(value => value * 2);

        // Assert
        matched.Should().Be(10);
    }

    [Fact]
    public void Match_GivenFailureAndTypedSuccessOnlyOverload_ShouldReturnDefault()
    {
        // Arrange
        var result = LithanResult<int>.Failure(new LithanError(1, "fail"));

        // Act
        var matched = result.Match(value => value * 2);

        // Assert
        matched.Should().Be(0);
    }

    [Fact]
    public void Match_GivenSuccessNullAndTypedOverload_ShouldReturnNullValuePath()
    {
        // Arrange
        var result = LithanResult<string?>.Success(null);

        // Act
        var matched = result.Match(success: _ => "success",
                                   failure: _ => "failure",
                                   nullValue: () => "null");

        // Assert
        matched.Should().Be("null");
    }

    [Fact]
    public async Task MatchAsync_GivenSuccess_ShouldExecuteExpectedPath()
    {
        // Arrange
        var dxcResult = LithanResult<bool>.Success(true);

        // Act
        await dxcResult.MatchAsync(success: async _ =>
                                            {
                                                Assert.True(true);
                                                return await Task.FromResult(true);
                                            },
                                   failure: async _ =>
                                            {
                                                Assert.Fail("Failure should not be called");
                                                return await Task.FromResult(false);
                                            });

        // Assert
    }

    [Fact]
    public async Task MatchAsync_GivenFailure_ShouldExecuteExpectedPath()
    {
        // Arrange
        var dxcResult = LithanResult<bool>.Failure(new LithanError(999, "Test Error"));

        // Act
        await dxcResult.MatchAsync(success: async _ =>
                                            {
                                                Assert.Fail("Success should not be called");
                                                return await Task.FromResult(true);
                                            },
                                   failure: async _ =>
                                            {
                                                Assert.True(true);
                                                return await Task.FromResult(false);
                                            });

        // Assert
    }

    [Fact]
    public async Task MatchAsync_GivenNull_ShouldExecuteExpectedPath()
    {
        // Arrange
        var dxcResult = LithanResult<string?>.Success(null);

        // Act
        await dxcResult.MatchAsync(success: async _ =>
                                            {
                                                Assert.Fail("Success should not be called");
                                                return await Task.FromResult("");
                                            },
                                   failure: async _ =>
                                            {
                                                Assert.Fail("Failure should not be called");
                                                return await Task.FromResult("");
                                            },
                                   nullValue: async () =>
                                              {
                                                  Assert.True(true);
                                                  return await Task.FromResult("null");
                                              });

        // Assert
    }

    [Fact]
    public async Task MatchAsync_GivenSuccessAndActionOverload_ShouldExecuteSuccessPath()
    {
        // Arrange
        var result        = LithanResult<string>.Success("ok");
        var successCalled = false;

        // Act
        await result.MatchAsync(success: async _ =>
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
    public async Task MatchAsync_GivenFailureAndActionOverload_ShouldExecuteFailurePath()
    {
        // Arrange
        var result        = LithanResult<string>.Failure(new LithanError(1, "fail"));
        var failureCalled = false;

        // Act
        await result.MatchAsync(success: async _ =>
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
    public async Task MatchAsync_GivenSuccessAndSuccessOnlyActionOverload_ShouldExecuteSuccessPath()
    {
        // Arrange
        var result        = LithanResult<string>.Success("ok");
        var successCalled = false;

        // Act
        await result.MatchAsync(async _ =>
                                {
                                    successCalled = true;
                                    await Task.CompletedTask;
                                });

        // Assert
        successCalled.Should().BeTrue();
    }

    [Fact]
    public async Task MatchAsync_GivenNullAndActionOverload_ShouldExecuteNullPath()
    {
        // Arrange
        var result     = LithanResult<string?>.Success(null);
        var nullCalled = false;

        // Act
        await result.MatchAsync(success: async _ =>
                                         {
                                             Assert.Fail("Success should not be called");
                                             await Task.CompletedTask;
                                         },
                                failure: async _ =>
                                         {
                                             Assert.Fail("Failure should not be called");
                                             await Task.CompletedTask;
                                         },
                                nullValue: async () =>
                                           {
                                               nullCalled = true;
                                               await Task.CompletedTask;
                                           });

        // Assert
        nullCalled.Should().BeTrue();
    }

    [Fact]
    public async Task MatchAsync_GivenSuccessAndTypedSuccessOnlyOverload_ShouldReturnSuccessValue()
    {
        // Arrange
        var result = LithanResult<int>.Success(4);

        // Act
        var matched = await result.MatchAsync(async value => await Task.FromResult(value * 2));

        // Assert
        matched.Should().Be(8);
    }

    [Fact]
    public async Task MatchAsync_GivenFailureAndTypedSuccessOnlyOverload_ShouldReturnDefault()
    {
        // Arrange
        var result = LithanResult<int>.Failure(new LithanError(1, "fail"));

        // Act
        var matched = await result.MatchAsync(async value => await Task.FromResult(value * 2));

        // Assert
        matched.Should().Be(0);
    }
}
