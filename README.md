# Lithan Core

Shared .NET libraries for Lithan applications — results/errors, ASP.NET Core API helpers, FluentValidation utilities, expression filters, and xUnit test helpers.

**Sponsored and created by [Lithan Solutions](https://www.lithan.co.za)**

Repository: [https://github.com/DracoRSA/lithan-core](https://github.com/DracoRSA/lithan-core)

## Packages

| Package | Description |
|---------|-------------|
| **Lithan.Core** | Core result and error types (`LithanResult`, `LithanResult<T>`, `LithanResults<T>`, `LithanError`) |
| **Lithan.Core.Application** | Application-layer helpers — `ExpressionExtensions` for dynamic filters and `LithanValidatorBase<T>` for FluentValidation + DataAnnotations |
| **Lithan.Core.Api** | ASP.NET Core helpers — `LithanControllerBase`, error/validation middleware, `ApiHealthCheck` |
| **Lithan.Core.TestUtilities** | Test construction helpers, random value generation, `MethodTestHelper`, `ThreadLocalRandom` |
| **Lithan.Core.TestUtilities.XUnit** | xUnit-focused helpers — constructor/property/method/attribute validation and assert extensions |

Target framework: **.NET 10**.

Package versions are published from `master` as `{major}.{minor}.{build}` (major/minor from the project file; patch from the CI build number).

```bash
dotnet add package Lithan.Core
dotnet add package Lithan.Core.Application
dotnet add package Lithan.Core.Api
dotnet add package Lithan.Core.TestUtilities
dotnet add package Lithan.Core.TestUtilities.XUnit
```

---

## How to use

### Lithan.Core — results and errors

```csharp
using Lithan.Core.Models;
using Lithan.Core.Result;

// Success / failure
var ok = LithanResult<string>.Success("created");
var fail = LithanResult<string>.Failure(new LithanError(1001, "Not found"));

ok.Match(
    success: value => Console.WriteLine(value),
    failure: error => Console.WriteLine(error.Message));

// Non-generic result
var done = LithanResult.Success();
done.Match(() => Console.WriteLine("ok"));

// Multiple values
var many = LithanResults<int>.Success([1, 2, 3]);
```

### Lithan.Core.Application — expressions and validation

**Dynamic filter expressions:**

```csharp
using Lithan.Core.Application.Extensions;

var (parameter, predicate) = typeof(Customer)
    .CreateEqualsCondition(nameof(Customer.Status), "Active");

predicate = predicate.AddContainsFilterCondition(parameter, nameof(Customer.Name), "lithan");

var filter = Expression.Lambda<Func<Customer, bool>>(predicate, parameter);
var matches = customers.AsQueryable().Where(filter);
```

**Validators that honour `[Required]` for value-type defaults** (`DateTime.MinValue`, `0`, blank strings, etc.):

```csharp
using FluentValidation;
using Lithan.Core.Application.Validation;

public sealed class CustomerValidator : LithanValidatorBase<Customer>
{
    public CustomerValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
    }
}
```

### Lithan.Core.Api — controllers, middleware, health

```csharp
using Lithan.Core.Api.Controllers;
using Lithan.Core.Api.Middleware;
using Lithan.Core.Api.HealthChecks;

public sealed class OrdersController : LithanControllerBase
{
    [HttpPost]
    public IActionResult Create(Order order)
    {
        return CreateCreatedResult(Request, $"orders/{order.Id}", order);
    }
}

// Program.cs
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<ValidationExceptionHandlingMiddleware>();

builder.Services.AddHealthChecks()
    .AddCheck("api", new ApiHealthCheck("Orders"));
```

### Lithan.Core.TestUtilities / XUnit — unit test helpers

```csharp
using Lithan.Core.TestUtilities;
using Lithan.Core.TestUtilities.XUnit;

[Theory]
[InlineData("message")]
public void Constructor_GivenNullParameterValue_ShouldThrowArgumentNullException(string parameterName)
{
    // Arrange

    // Act
    XUnitConstructorTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<LithanError>(parameterName);

    // Assert
}

[Fact]
public void Method_GivenNullArgument_ShouldThrowArgumentNullException()
{
    // Arrange

    // Act
    MethodTestHelper.ValidateArgumentNullExceptionIsThrownIfParameterIsNull<OrderService>(
        nameof(OrderService.Create), "order");

    // Assert
}

[Fact]
public void Property_GivenValue_ShouldRoundTrip()
{
    // Arrange

    // Act
    XUnitPropertyTestHelper.ValidateGetAndSet<Customer>(nameof(Customer.Name));

    // Assert
}
```

---

## Building and testing

```bash
dotnet build src/Lithan.Core.slnx
dotnet test src/Lithan.Core.slnx
```

## License

See [LICENSE](LICENSE).

## About Lithan Solutions

These packages are developed and sponsored by **[Lithan Solutions](https://www.lithan.co.za)** — visit [www.lithan.co.za](https://www.lithan.co.za) for more information.
