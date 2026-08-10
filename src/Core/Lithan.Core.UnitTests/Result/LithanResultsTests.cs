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
}