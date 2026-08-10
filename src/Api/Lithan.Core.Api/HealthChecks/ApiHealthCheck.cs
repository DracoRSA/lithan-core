using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Lithan.Core.Api.HealthChecks;

/// <summary>
/// API Health Check to verify that the API is running.
/// </summary>
public class ApiHealthCheck : IHealthCheck
{
    private readonly string _apiName;

    public ApiHealthCheck(string apiName)
    {
        _apiName = apiName ?? throw new ArgumentNullException(nameof(apiName));
    }

    /// <inheritdoc />
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, 
                                                    CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.FromResult(HealthCheckResult.Healthy($"{_apiName} API is healthy"));
    }
}