using FluentValidation;
using Xunit;
using AwesomeAssertions;
using Lithan.Core.Application.Validation;
using Lithan.Core.Application.UnitTests.Fakes;

namespace Lithan.Core.Application.UnitTests.Validation;

public class LithanValidatorBaseTests
{
    private readonly TestValidator _validator = new();

    [Fact]
    public void Validate_GivenValidModel_ShouldNotHaveErrors()
    {
        // Arrange
        var model = new FakeRequiredModel
                    {
                        Name      = "Lithan",
                        CreatedOn = DateTime.UtcNow,
                        Count     = 1,
                        Amount    = 1L,
                        Ratio     = 1.5d
                    };

        // Act
        var result = _validator.Validate(model);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_GivenMissingRequiredString_ShouldFailValidation()
    {
        // Arrange
        var model = new FakeRequiredModel
                    {
                        Name      = " ",
                        CreatedOn = DateTime.UtcNow,
                        Count     = 1,
                        Amount    = 1L,
                        Ratio     = 1.5d
                    };

        // Act
        var result = _validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(FakeRequiredModel.Name) &&
                                                error.ErrorMessage == "Name is required");
    }

    [Fact]
    public void Validate_GivenDefaultDateTime_ShouldFailRequiredValidation()
    {
        // Arrange
        var model = new FakeRequiredModel
                    {
                        Name      = "Lithan",
                        CreatedOn = DateTime.MinValue,
                        Count     = 1,
                        Amount    = 1L,
                        Ratio     = 1.5d
                    };

        // Act
        var result = _validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(FakeRequiredModel.CreatedOn) &&
                                                error.ErrorMessage == "CreatedOn is required");
    }

    [Fact]
    public void Validate_GivenDefaultInt_ShouldFailRequiredValidation()
    {
        // Arrange
        var model = new FakeRequiredModel
                    {
                        Name      = "Lithan",
                        CreatedOn = DateTime.UtcNow,
                        Count     = 0,
                        Amount    = 1L,
                        Ratio     = 1.5d
                    };

        // Act
        var result = _validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(FakeRequiredModel.Count) &&
                                                error.ErrorMessage == "Count is required");
    }

    [Fact]
    public void Validate_GivenDefaultLong_ShouldFailRequiredValidation()
    {
        // Arrange
        var model = new FakeRequiredModel
                    {
                        Name      = "Lithan",
                        CreatedOn = DateTime.UtcNow,
                        Count     = 1,
                        Amount    = 0L,
                        Ratio     = 1.5d
                    };

        // Act
        var result = _validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(FakeRequiredModel.Amount));
    }

    [Fact]
    public void Validate_GivenDefaultDouble_ShouldFailRequiredValidation()
    {
        // Arrange
        var model = new FakeRequiredModel
                    {
                        Name      = "Lithan",
                        CreatedOn = DateTime.UtcNow,
                        Count     = 1,
                        Amount    = 1L,
                        Ratio     = 0d
                    };

        // Act
        var result = _validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(FakeRequiredModel.Ratio));
    }

    [Fact]
    public void Validate_GivenNullModel_ShouldThrowInvalidOperationException()
    {
        // Arrange

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => _validator.Validate((FakeRequiredModel)null!));

        // Assert
        exception.Message.Should().Contain("null model");
    }

    private sealed class TestValidator : LithanValidatorBase<FakeRequiredModel>
    {
    }
}
