using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FluentValidation;

namespace Lithan.Core.Api.Middleware;

/// <summary>
/// Validation exception handling middleware.
/// </summary>
public sealed class ValidationExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ValidationExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Validation Exception Handling Middleware constructor
    /// </summary>
    /// <param name="next">Next Request delegate</param>
    /// <param name="logger">Logger</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ValidationExceptionHandlingMiddleware(RequestDelegate next,
                                                 ILogger<ValidationExceptionHandlingMiddleware> logger)
    {
        _next   = next;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Invokes the middleware to handle validation exceptions
    /// </summary>
    /// <param name="context">HTTP Context</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException validationException)
        {
            _logger.Log(LogLevel.Error, validationException, "Validation Error occurred");

            var problemDetails = new ProblemDetails
                                 {
                                     Status = StatusCodes.Status422UnprocessableEntity,
                                     Type   = "ValidationFailure",
                                     Title  = "Validation error",
                                     Detail = "One or more validation errors has occurred"
                                 };

            if (validationException.Errors is not null)
            {
                problemDetails.Extensions["errors"] = validationException.Errors;
            }

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}