using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Lithan.Core.Api.Middleware;

/// <summary>
/// Error Handler Middleware
/// </summary>
public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    /// <summary>
    /// Error Handler Middleware constructor
    /// </summary>
    /// <param name="next">Next Request delegate</param>
    /// <param name="logger">Logger</param>
    public ErrorHandlerMiddleware(RequestDelegate next,
                                  ILogger<ErrorHandlerMiddleware> logger)
    {
        _next   = next;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Invokes the middleware to handle exceptions
    /// </summary>
    /// <param name="context">HTTP Context</param>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception runtimeException)
        {
            _logger.Log(LogLevel.Error, runtimeException, "Runtime Exception occurred");

            var errorDetails = JsonSerializer.Serialize(new
                                                        {
                                                            message = "Something went wrong. Please contact technical support.",
                                                            details = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                                                                          ? runtimeException.ToString()
                                                                          : null
                                                        });

            var problemDetails = new ProblemDetails
                                 {
                                     Status = StatusCodes.Status500InternalServerError,
                                     Type   = "Server Error",
                                     Title  = "Server Error",
                                     Detail = errorDetails
                                 };

            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode  = (int)HttpStatusCode.InternalServerError;

            await response.WriteAsJsonAsync(problemDetails);
        }
    }
}