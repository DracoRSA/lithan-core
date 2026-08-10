using Xunit;
using AwesomeAssertions;
using Lithan.Core.Result;
using Lithan.Core.Models;

namespace Lithan.Core.UnitTests.Result;

public class LithanResultsTests
{
    [Fact]
    public void Constructor()
    {
        // Arrange

        // Act
        var dxcResult = new LithanResults<bool>(new List<bool>());

        // Assert
        dxcResult.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_GivenSuccess_ShouldSetPropertiesToExpectedValues()
    {
        // Arrange

        // Act
        var dxcResult = new LithanResults<bool>(new List<bool>());

        // Assert
        dxcResult.IsError.Should().BeFalse();
        dxcResult.IsSuccess.Should().BeTrue();
        dxcResult.Values.Should().NotBeNull();
        dxcResult.Error.Should().BeNull();
    }

    [Fact]
    public void Constructor_GivenError_ShouldSetPropertiesToExpectedValues()
    {
        // Arrange
        var resultErrors = new LithanError(1, "Test Error Message");

        // Act
        var dxcResult = new LithanResults<bool>(resultErrors);

        // Assert
        dxcResult.IsError.Should().BeTrue();
        dxcResult.IsSuccess.Should().BeFalse();
        dxcResult.Values.Should().BeNull();
        dxcResult.Error.Should().BeEquivalentTo(resultErrors);
    }

    [Fact]
    public void Success_ShouldSetPropertiesToExpectedValues()
    {
        // Arrange

        // Act
        var dxcResult = LithanResults<bool>.Success(new List<bool>());

        // Assert
        dxcResult.IsError.Should().BeFalse();
        dxcResult.IsSuccess.Should().BeTrue();
        dxcResult.Values.Should().NotBeNull();
        dxcResult.Error.Should().BeNull();
    }

    [Fact]
    public void Failure_ShouldSetPropertiesToExpectedValues()
    {
        // Arrange
        var resultErrors = new LithanError(1, "Test Error Message");

        // Act
        var dxcResult = LithanResults<bool>.Failure(resultErrors);

        // Assert
        dxcResult.IsError.Should().BeTrue();
        dxcResult.IsSuccess.Should().BeFalse();
        dxcResult.Values.Should().BeNull();
        dxcResult.Error.Should().BeEquivalentTo(resultErrors);
    }

    [Fact]
    public void ImplicitOperator_GivenValues_ShouldCreateSuccessResult()
    {
        // Arrange
        var values = new List<string> { "a", "b" };

        // Act
        LithanResults<string> result = values;

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Values.Should().BeSameAs(values);
    }

    [Fact]
    public void ImplicitOperator_GivenError_ShouldCreateFailureResult()
    {
        // Arrange
        var resultError = new LithanError(3, "error");

        // Act
        LithanResults<string> result = resultError;

        // Assert
        result.IsError.Should().BeTrue();
        result.Error.Should().Be(resultError);
    }

    [Fact]
    public void Match_GivenSuccess_ShouldExecuteExpectedPath()
    {
        // Arrange
        var dxcResults = LithanResults<string>.Success(new List<string>());

        // Act
        dxcResults.Match(success: _ =>
                                  {
                                      Assert.True(true);
                                      return string.Empty;
                                  },
                         failure: _ =>
                                  {
                                      Assert.Fail("Failure should not be called");
                                      return string.Empty;
                                  });

        // Assert
    }

    [Fact]
    public void Match_GivenFailure_ShouldExecuteExpectedPath()
    {
        // Arrange
        var dxcResults = LithanResults<string>.Failure(new LithanError(999, "Test Error"));

        // Act
        dxcResults.Match(success: _ =>
                                  {
                                      Assert.Fail("Success should not be called");
                                      return string.Empty;
                                  },
                         failure: _ =>
                                  {
                                      Assert.True(true);
                                      return string.Empty;
                                  });

        // Assert
    }

    [Fact]
    public void Match_GivenSuccessWithNull_ShouldExecuteExpectedPath()
    {
        // Arrange
        var dxcResults = LithanResults<string>.Success(null);

        // Act
        dxcResults.Match<string>(success: _ =>
                                          {
                                              Assert.Fail("Success should not be called");
                                              return null;
                                          },
                                 failure: _ =>
                                          {
                                              Assert.Fail("Failure should not be called");
                                              return null;
                                          },
                                 nullValue: () =>
                                            {
                                                Assert.True(true);
                                                return null!;
                                            });

        // Assert
    }

    [Fact]
    public void Match_GivenSuccessAndSuccessOnlyOverload_ShouldExecuteSuccessPath()
    {
        // Arrange
        var result        = LithanResults<string>.Success(["a"]);
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
        var result        = LithanResults<string>.Failure(new LithanError(1, "fail"));
        var successCalled = false;

        // Act
        result.Match(_ => successCalled = true);

        // Assert
        successCalled.Should().BeFalse();
    }

    [Fact]
    public void Match_GivenSuccessAndActionOverload_ShouldExecuteSuccessPath()
    {
        // Arrange
        var result        = LithanResults<string>.Success(["a"]);
        var successCalled = false;

        // Act
        result.Match(success: _ => successCalled = true,
                     failure: _ => Assert.Fail("Failure should not be called"));

        // Assert
        successCalled.Should().BeTrue();
    }

    [Fact]
    public void Match_GivenNullAndActionOverload_ShouldExecuteNullPath()
    {
        // Arrange
        var result     = LithanResults<string>.Success(null);
        var nullCalled = false;

        // Act
        result.Match(success: _ => Assert.Fail("Success should not be called"),
                     failure: _ => Assert.Fail("Failure should not be called"),
                     nullValue: () => nullCalled = true);

        // Assert
        nullCalled.Should().BeTrue();
    }

    [Fact]
    public void Match_GivenSuccessAndTypedSuccessOnlyOverload_ShouldReturnSuccessValue()
    {
        // Arrange
        var result = LithanResults<int>.Success([1, 2, 3]);

        // Act
        var matched = result.Match(values => values.Count);

        // Assert
        matched.Should().Be(3);
    }

    [Fact]
    public void Match_GivenFailureAndTypedSuccessOnlyOverload_ShouldReturnDefault()
    {
        // Arrange
        var result = LithanResults<int>.Failure(new LithanError(1, "fail"));

        // Act
        var matched = result.Match(values => values.Count);

        // Assert
        matched.Should().Be(0);
    }

    [Fact]
    public async Task MatchAsync_GivenSuccess_ShouldExecuteExpectedPath()
    {
        // Arrange
        var dxcResults = LithanResults<string>.Success(new List<string>());

        // Act
        await dxcResults.MatchAsync(success: _ =>
                                             {
                                                 Assert.True(true);
                                                 return Task.CompletedTask;
                                             },
                                    failure: _ =>
                                             {
                                                 Assert.Fail("Failure should not be called");
                                                 return Task.CompletedTask;
                                             });

        // Assert
    }

    [Fact]
    public async Task MatchAsync_GivenFailure_ShouldExecuteExpectedPath()
    {
        // Arrange
        var dxcResults = LithanResults<string>.Failure(new LithanError(999, "Test error"));

        // Act
        await dxcResults.MatchAsync(success: _ =>
                                             {
                                                 Assert.Fail("Success should not be called");
                                                 return Task.CompletedTask;
                                             },
                                    failure: _ =>
                                             {
                                                 Assert.True(true);
                                                 return Task.CompletedTask;
                                             });

        // Assert
    }

    [Fact]
    public async Task MatchAsync_GivenSuccessWithNull_ShouldExecuteExpectedPath()
    {
        // Arrange
        var dxcResults = LithanResults<string?>.Success(null);

        // Act
        await dxcResults.MatchAsync(success: _ =>
                                             {
                                                 Assert.Fail("Success should not be called");
                                                 return Task.CompletedTask;
                                             },
                                    failure: _ =>
                                             {
                                                 Assert.Fail("Failure should not be called");
                                                 return Task.CompletedTask;
                                             },
                                    nullValue: () =>
                                               {
                                                   Assert.True(true);
                                                   return Task.CompletedTask;
                                               });

        // Assert
    }

    [Fact]
    public async Task MatchAsync_GivenSuccessAndSuccessOnlyOverload_ShouldExecuteSuccessPath()
    {
        // Arrange
        var result        = LithanResults<string>.Success(["a"]);
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
    public async Task MatchAsync_GivenSuccessAndTypedOverload_ShouldReturnSuccessValue()
    {
        // Arrange
        var result = LithanResults<int>.Success([1, 2]);

        // Act
        var matched = await result.MatchAsync(success: async values => await Task.FromResult(values.Count),
                                              failure: async _ => await Task.FromResult(-1));

        // Assert
        matched.Should().Be(2);
    }

    [Fact]
    public async Task MatchAsync_GivenFailureAndTypedOverload_ShouldReturnFailureValue()
    {
        // Arrange
        var result = LithanResults<int>.Failure(new LithanError(1, "fail"));

        // Act
        var matched = await result.MatchAsync(success: async values => await Task.FromResult(values.Count),
                                              failure: async _ => await Task.FromResult(-1));

        // Assert
        matched.Should().Be(-1);
    }

    [Fact]
    public async Task MatchAsync_GivenNullAndTypedOverload_ShouldReturnNullValue()
    {
        // Arrange
        var result = LithanResults<int>.Success(null);

        // Act
        var matched = await result.MatchAsync(success: async values => await Task.FromResult(values.Count),
                                              failure: async _ => await Task.FromResult(-1),
                                              nullValue: async () => await Task.FromResult(0));

        // Assert
        matched.Should().Be(0);
    }

    [Fact]
    public async Task MatchAsync_GivenSuccessAndTypedSuccessOnlyOverload_ShouldReturnSuccessValue()
    {
        // Arrange
        var result = LithanResults<int>.Success([1, 2, 3]);

        // Act
        var matched = await result.MatchAsync(async values => await Task.FromResult(values.Sum()));

        // Assert
        matched.Should().Be(6);
    }

    [Fact]
    public async Task MatchAsync_GivenFailureAndTypedSuccessOnlyOverload_ShouldReturnDefault()
    {
        // Arrange
        var result = LithanResults<int>.Failure(new LithanError(1, "fail"));

        // Act
        var matched = await result.MatchAsync(async values => await Task.FromResult(values.Sum()));

        // Assert
        matched.Should().Be(0);
    }
}
