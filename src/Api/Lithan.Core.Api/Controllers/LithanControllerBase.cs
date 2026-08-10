using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Lithan.Core.Api.Controllers;

/// <summary>
/// Controller Base Class
/// </summary>
public abstract class LithanControllerBase : Controller
{
    /// <summary>
    /// Create an CreatedResult with a relative location path and the returned object.
    /// </summary>
    /// <param name="httpRequest">HttpRequest</param>
    /// <param name="relativeLocationPath">Relative location path</param>
    /// <param name="returnedObject">Returned object</param>
    /// <typeparam name="T">Return Object Type</typeparam>
    /// <returns>
    /// A newly created <see cref="CreatedResult"/> with the specified relative location path and returned object.
    /// </returns>
    protected CreatedResult CreateCreatedResult<T>(HttpRequest? httpRequest,
                                                   string relativeLocationPath,
                                                   T returnedObject)
        where T : class
    {
        if (httpRequest == null)
        {
            return new CreatedResult(new Uri($"https://locahost/api/{relativeLocationPath}"), returnedObject);
        }

        if (httpRequest.Headers.TryGetValue("X-ORIGINAL-HOST", out var originalHostHeader))
        {
            return new CreatedResult(new Uri($"https://{originalHostHeader}/{httpRequest.PathBase}/{relativeLocationPath}"), returnedObject);
        }

        return new CreatedResult(new Uri($"https://{httpRequest.Host}/{httpRequest.PathBase}/{relativeLocationPath}"), returnedObject);
    }

    /// <summary>
    /// Create a Conflict Result
    /// </summary>
    /// <param name="errorCode">Error Code</param>
    /// <param name="errorMessage">Error Message</param>
    /// <returns></returns>
    protected ActionResult CreateConflictResult(int errorCode, string errorMessage)
    {
        var errorDetails = JsonSerializer.Serialize(new
        {
            message = $"[{errorCode}] {errorMessage}",
            details = errorMessage
        });

        return StatusCode(StatusCodes.Status409Conflict,
                          new ProblemDetails
                          {
                              Status = StatusCodes.Status409Conflict,
                              Type   = "Server Error",
                              Title  = "Server Error",
                              Detail = errorDetails
                          });
    }

    /// <summary>
    /// Create a Bad Request Result
    /// </summary>
    /// <param name="errorCode">Error Code</param>
    /// <param name="errorMessage">Error Message</param>
    /// <returns></returns>
    protected ActionResult CreateBadRequestResult(int errorCode, string errorMessage)
    {
        var errorDetails = JsonSerializer.Serialize(new
        {
            message = $"[{errorCode}] {errorMessage}",
            details = errorMessage
        });

        return StatusCode(StatusCodes.Status400BadRequest,
                          new ProblemDetails
                          {
                              Status = StatusCodes.Status400BadRequest,
                              Type   = "Server Error",
                              Title  = "Server Error",
                              Detail = errorDetails
                          });
    }

    /// <summary>
    /// Create Internal Server Error Result
    /// </summary>
    /// <param name="errorCode">Error Code</param>
    /// <param name="errorMessage">Error Message</param>
    /// <param name="runtimeException">Runtime Exception (Optional)</param>
    /// <returns>Internal Server Error Object Result</returns>
    protected ObjectResult CreateInternalServerErrorResult(int errorCode, string errorMessage, Exception? runtimeException = null)
    {
        var errorDetails = JsonSerializer.Serialize(new
        {
            message = $"[{errorCode}] {errorMessage}",
            details = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                          ? runtimeException?.ToString() ?? "Unknown error occurred"
                          : null
        });

        return StatusCode(StatusCodes.Status500InternalServerError,
                          new ProblemDetails
                          {
                              Status = StatusCodes.Status500InternalServerError,
                              Type   = "Server Error",
                              Title  = "Server Error",
                              Detail = errorDetails
                          });
    }
}