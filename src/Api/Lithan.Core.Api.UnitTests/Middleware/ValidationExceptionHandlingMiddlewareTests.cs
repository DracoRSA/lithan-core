using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using AwesomeAssertions;
using NSubstitute;
using Lithan.Core.Api.Middleware;
using Lithan.Core.TestUtilities.XUnit;

namespace Lithan.Core.Api.UnitTests.Middleware;

public class ValidationExceptionHandlingMiddlewareTests
{
    [Theory]
    [InlineData("logger")]
    public void Constructor_GivenNullParameterValue_ShouldThrowArgumentNullException(string parameterName)
    {
        // Arrange

        // Act
        XUnitConstructorTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<ValidationExceptionHandlingMiddleware>(parameterName, ("next", (RequestDelegate)(_ => Task.CompletedTask)));

        // Assert
    }

    [Fact]
    public async Task InvokeAsync_GivenNoException_ShouldCallNextDelegate()
    {
        // Arrange
        var nextCalled = false;
        var logger     = Substitute.For<ILogger<ValidationExceptionHandlingMiddleware>>();
        var middleware = new ValidationExceptionHandlingMiddleware(_ =>
                                                                   {
                                                                       nextCalled = true;
                                                                       return Task.CompletedTask;
                                                                   },
                                                                   logger);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task InvokeAsync_GivenValidationException_ShouldWriteBadRequestProblemDetails()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ValidationExceptionHandlingMiddleware>>();
        var failures = new List<ValidationFailure>
                       {
                           new("Name", "Name is required")
                       };
        var middleware = new ValidationExceptionHandlingMiddleware(_ => throw new ValidationException(failures), logger);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var document = await JsonDocument.ParseAsync(context.Response.Body, cancellationToken: TestContext.Current.CancellationToken);
        document.RootElement.GetProperty("title").GetString().Should().Be("Validation error");
        document.RootElement.GetProperty("type").GetString().Should().Be("ValidationFailure");
        document.RootElement.GetProperty("status").GetInt32().Should().Be(StatusCodes.Status422UnprocessableEntity);
        document.RootElement.TryGetProperty("errors", out _).Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_GivenNonValidationException_ShouldPropagateException()
    {
        // Arrange
        var logger     = Substitute.For<ILogger<ValidationExceptionHandlingMiddleware>>();
        var middleware = new ValidationExceptionHandlingMiddleware(_ => throw new InvalidOperationException("boom"), logger);
        var context    = new DefaultHttpContext();

        // Act
        var exception = await Record.ExceptionAsync(() => middleware.InvokeAsync(context));

        // Assert
        exception.Should().BeOfType<InvalidOperationException>().Which.Message.Should().Be("boom");
    }
}
