using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using AwesomeAssertions;
using NSubstitute;
using Lithan.Core.Api.Middleware;
using Lithan.Core.TestUtilities.XUnit;

namespace Lithan.Core.Api.UnitTests.Middleware;

public class ErrorHandlerMiddlewareTests
{
    [Theory]
    [InlineData("logger")]
    public void Constructor_GivenNullParameterValue_ShouldThrowArgumentNullException(string parameterName)
    {
        // Arrange

        // Act
        XUnitConstructorTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<ErrorHandlerMiddleware>(parameterName, ("next", (RequestDelegate)(_ => Task.CompletedTask)));

        // Assert
    }

    [Fact]
    public async Task Invoke_GivenNoException_ShouldCallNextDelegate()
    {
        // Arrange
        var nextCalled = false;
        var logger     = Substitute.For<ILogger<ErrorHandlerMiddleware>>();
        var middleware = new ErrorHandlerMiddleware(_ =>
                                                    {
                                                        nextCalled = true;
                                                        return Task.CompletedTask;
                                                    },
                                                    logger);
        var context = new DefaultHttpContext();

        // Act
        await middleware.Invoke(context);

        // Assert
        nextCalled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task Invoke_GivenExceptionAndNonDevelopmentEnvironment_ShouldWriteServerErrorWithoutDetails()
    {
        // Arrange
        var previousEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");
        var logger     = Substitute.For<ILogger<ErrorHandlerMiddleware>>();
        var middleware = new ErrorHandlerMiddleware(_ => throw new InvalidOperationException("secret failure"), logger);
        var context    = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        try
        {
            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            context.Response.ContentType.Should().StartWith("application/json");
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var document = await JsonDocument.ParseAsync(context.Response.Body, cancellationToken: TestContext.Current.CancellationToken);
            document.RootElement.GetProperty("title").GetString().Should().Be("Server Error");
            document.RootElement.GetProperty("status").GetInt32().Should().Be(StatusCodes.Status500InternalServerError);

            using var detailDocument = JsonDocument.Parse(document.RootElement.GetProperty("detail").GetString()!);
            detailDocument.RootElement.GetProperty("message").GetString().Should().Be("Something went wrong. Please contact technical support.");
            detailDocument.RootElement.GetProperty("details").ValueKind.Should().Be(JsonValueKind.Null);
        }
        finally
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", previousEnvironment);
        }
    }

    [Fact]
    public async Task Invoke_GivenExceptionAndDevelopmentEnvironment_ShouldIncludeExceptionDetails()
    {
        // Arrange
        var previousEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        var logger     = Substitute.For<ILogger<ErrorHandlerMiddleware>>();
        var middleware = new ErrorHandlerMiddleware(_ => throw new InvalidOperationException("visible failure"), logger);
        var context    = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        try
        {
            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var document = await JsonDocument.ParseAsync(context.Response.Body, cancellationToken: TestContext.Current.CancellationToken);
            using var detailDocument = JsonDocument.Parse(document.RootElement.GetProperty("detail").GetString()!);
            detailDocument.RootElement.GetProperty("details").GetString().Should().Contain("visible failure");
        }
        finally
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", previousEnvironment);
        }
    }
}
