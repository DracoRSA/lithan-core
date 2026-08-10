using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;
using AwesomeAssertions;
using Lithan.Core.Api.HealthChecks;
using Lithan.Core.TestUtilities.XUnit;

namespace Lithan.Core.Api.UnitTests.HealthChecks;

public class ApiHealthCheckTests
{
    [Theory]
    [InlineData("apiName")]
    public void Constructor_GivenNullParameterValue_ShouldThrowArgumentNullException(string parameterName)
    {
        // Arrange

        // Act
        XUnitConstructorTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<ApiHealthCheck>(parameterName);

        // Assert
    }

    [Fact]
    public void Constructor_GivenApiName_ShouldCreateInstance()
    {
        // Arrange

        // Act
        var healthCheck = new ApiHealthCheck("Lithan");

        // Assert
        healthCheck.Should().NotBeNull();
    }

    [Fact]
    public async Task CheckHealthAsync_ShouldReturnHealthyResultWithApiName()
    {
        // Arrange
        var apiName     = "Lithan";
        var healthCheck = new ApiHealthCheck(apiName);
        var context     = new HealthCheckContext();

        // Act
        var result = await healthCheck.CheckHealthAsync(context, TestContext.Current.CancellationToken);

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
        result.Description.Should().Be($"{apiName} API is healthy");
    }
}
