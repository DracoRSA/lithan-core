using System.Reflection;
using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Lithan.Core.Application.Validation;

/// <summary>
/// Lithan Validator Base
/// </summary>
/// <typeparam name="T">Data Type to be validated</typeparam>
public abstract class LithanValidatorBase<T> : AbstractValidator<T>
{
    private readonly Dictionary<Type, Func<object, bool>> _validators = new()
    {
        { typeof(DateTime), o => (DateTime)o == DateTime.MinValue },
        { typeof(string), o => string.IsNullOrWhiteSpace((string)o) },
        { typeof(int), o => (int)o           == 0 },
        { typeof(long), o => (long)o         == 0L },
        { typeof(double), o => (double)o     == 0d }
    };

    /// <inheritdoc />
    protected LithanValidatorBase()
    {
        RuleFor(x => x)
            .Custom((model, context) =>
                    {
                        var results = new List<ValidationResult>();
                        if (model != null)
                        {
                            var validationContext = new ValidationContext(model);

                            Validator.TryValidateObject(model,
                                                        validationContext,
                                                        results,
                                                        validateAllProperties: true);

                            // Extend [Required] semantics for common value types.
                            // DataAnnotations [Required] does not fail for default value types
                            // like DateTime.MinValue or numeric zero.
                            var properties = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                            foreach (var property in properties)
                            {
                                var requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();
                                if (requiredAttribute == null)
                                {
                                    continue;
                                }

                                // Skip custom checks if validation already has an error for this member.
                                if (results.Any(r => r.MemberNames.Any(member => member == property.Name)))
                                {
                                    continue;
                                }

                                var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                var errorMessage = requiredAttribute.ErrorMessage                    ?? $"{property.Name} is required";

                                var value = property.GetValue(model);

                                // Validate the different Types with their default values.
                                var typeValidator = _validators.FirstOrDefault(pair => pair.Key == propertyType);
                                if (typeValidator.Value is not null &&
                                    typeValidator.Value(value!))
                                {
                                    results.Add(new ValidationResult(errorMessage, [property.Name]));
                                    continue;
                                }

                                if (value is null)
                                {
                                    results.Add(new ValidationResult(errorMessage, [property.Name]));
                                }
                            }
                        }

                        foreach (var result in results)
                        {
                            context.AddFailure(result.MemberNames.FirstOrDefault() ?? string.Empty, result.ErrorMessage);
                        }
                    });
    }
}