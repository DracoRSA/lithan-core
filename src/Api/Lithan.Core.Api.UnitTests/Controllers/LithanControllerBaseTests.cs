using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using AwesomeAssertions;
using Lithan.Core.Api.Controllers;

namespace Lithan.Core.Api.UnitTests.Controllers;

public class LithanControllerBaseTests
{
    private readonly TestableLithanController _controller = new();

    [Fact]
    public void CreateCreatedResult_GivenNullHttpRequest_ShouldUseLocalhostUri()
    {
        // Arrange
        var returnedObject = new { Id = 1 };

        // Act
        var result = _controller.CreateCreatedResultPublic(null, "resources/1", returnedObject);

        // Assert
        result.Should().NotBeNull();
        result.Location.Should().Be("https://locahost/api/resources/1");
        result.Value.Should().BeSameAs(returnedObject);
    }

    [Fact]
    public void CreateCreatedResult_GivenOriginalHostHeader_ShouldUseOriginalHost()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-ORIGINAL-HOST"] = "gateway.example.com";
        httpContext.Request.Host = new HostString("internal.example.com");
        var returnedObject = new { Id = 7 };

        // Act
        var result = _controller.CreateCreatedResultPublic(httpContext.Request, "resources/7", returnedObject);

        // Assert
        result.Location.Should().Be("https://gateway.example.com/api/resources/7");
        result.Value.Should().BeSameAs(returnedObject);
    }

    [Fact]
    public void CreateCreatedResult_GivenHttpRequestWithoutOriginalHost_ShouldUseRequestHost()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Host = new HostString("api.example.com");
        var returnedObject = new { Id = 3 };

        // Act
        var result = _controller.CreateCreatedResultPublic(httpContext.Request, "resources/3", returnedObject);

        // Assert
        result.Location.Should().Be("https://api.example.com/api/resources/3");
        result.Value.Should().BeSameAs(returnedObject);
    }

    [Fact]
    public void CreateConflictResult_ShouldReturnConflictProblemDetails()
    {
        // Arrange
        var errorCode    = 4091;
        var errorMessage = "Conflict occurred";

        // Act
        var result = _controller.CreateConflictResultPublic(errorCode, errorMessage);

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status409Conflict);

        var problemDetails = objectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Status.Should().Be(StatusCodes.Status409Conflict);
        problemDetails.Type.Should().Be("Server Error");
        problemDetails.Title.Should().Be("Server Error");
        problemDetails.Detail.Should().Contain($"[{errorCode}] {errorMessage}");
        problemDetails.Detail.Should().Contain(errorMessage);
    }

    [Fact]
    public void CreateBadRequestResult_ShouldReturnBadRequestProblemDetails()
    {
        // Arrange
        var errorCode    = 4001;
        var errorMessage = "Bad request occurred";

        // Act
        var result = _controller.CreateBadRequestResultPublic(errorCode, errorMessage);

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        var problemDetails = objectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Status.Should().Be(StatusCodes.Status400BadRequest);
        problemDetails.Detail.Should().Contain($"[{errorCode}] {errorMessage}");
    }

    [Fact]
    public void CreateInternalServerErrorResult_GivenNonDevelopmentEnvironment_ShouldOmitExceptionDetails()
    {
        // Arrange
        var previousEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");
        var runtimeException = new InvalidOperationException("secret failure");

        try
        {
            // Act
            var result = _controller.CreateInternalServerErrorResultPublic(5001, "Server failed", runtimeException);

            // Assert
            var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            var problemDetails = objectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
            using var document = JsonDocument.Parse(problemDetails.Detail!);
            document.RootElement.GetProperty("message").GetString().Should().Be("[5001] Server failed");
            document.RootElement.GetProperty("details").ValueKind.Should().Be(JsonValueKind.Null);
        }
        finally
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", previousEnvironment);
        }
    }

    [Fact]
    public void CreateInternalServerErrorResult_GivenDevelopmentEnvironment_ShouldIncludeExceptionDetails()
    {
        // Arrange
        var previousEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        var runtimeException = new InvalidOperationException("visible failure");

        try
        {
            // Act
            var result = _controller.CreateInternalServerErrorResultPublic(5002, "Server failed", runtimeException);

            // Assert
            var problemDetails = result.Value.Should().BeOfType<ProblemDetails>().Subject;
            using var document = JsonDocument.Parse(problemDetails.Detail!);
            document.RootElement.GetProperty("details").GetString().Should().Contain("visible failure");
        }
        finally
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", previousEnvironment);
        }
    }

    [Fact]
    public void CreateInternalServerErrorResult_GivenDevelopmentEnvironmentAndNoException_ShouldUseUnknownError()
    {
        // Arrange
        var previousEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

        try
        {
            // Act
            var result = _controller.CreateInternalServerErrorResultPublic(5003, "Server failed");

            // Assert
            var problemDetails = result.Value.Should().BeOfType<ProblemDetails>().Subject;
            using var document = JsonDocument.Parse(problemDetails.Detail!);
            document.RootElement.GetProperty("details").GetString().Should().Be("Unknown error occurred");
        }
        finally
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", previousEnvironment);
        }
    }

    private sealed class TestableLithanController : LithanControllerBase
    {
        public CreatedResult CreateCreatedResultPublic<T>(HttpRequest? httpRequest, string relativeLocationPath, T returnedObject)
            where T : class
            => CreateCreatedResult(httpRequest, relativeLocationPath, returnedObject);

        public ActionResult CreateConflictResultPublic(int errorCode, string errorMessage)
            => CreateConflictResult(errorCode, errorMessage);

        public ActionResult CreateBadRequestResultPublic(int errorCode, string errorMessage)
            => CreateBadRequestResult(errorCode, errorMessage);

        public ObjectResult CreateInternalServerErrorResultPublic(int errorCode, string errorMessage, Exception? runtimeException = null)
            => CreateInternalServerErrorResult(errorCode, errorMessage, runtimeException);
    }
}
