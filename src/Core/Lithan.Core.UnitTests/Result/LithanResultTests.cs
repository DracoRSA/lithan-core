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
                                                  return await Task.FromResult(ToString()!);
                                              });
    }
}