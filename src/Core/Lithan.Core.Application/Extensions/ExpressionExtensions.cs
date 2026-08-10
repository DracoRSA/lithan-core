using System.Linq.Expressions;

namespace Lithan.Core.Application.Extensions;

public static class ExpressionExtensions
{
    public static (ParameterExpression Parameter, Expression Predicate) CreateEqualsCondition<TProperty>(this Type type, string propertyName, TProperty value)
    {
        var parameter          = Expression.Parameter(type, "e");
        var propertyExpression = CreatePropertyExpression(parameter, propertyName);
        var predicate          = CreateComparisonExpression(propertyExpression, value, ExpressionType.Equal);
        return (parameter, predicate);
    }

    public static (ParameterExpression Parameter, Expression Predicate) CreateGreaterThanOrEqualCondition<TProperty>(this Type type, string propertyName, TProperty value)
    {
        var parameter          = Expression.Parameter(type, "e");
        var propertyExpression = CreatePropertyExpression(parameter, propertyName);
        var predicate          = CreateComparisonExpression(propertyExpression, value, ExpressionType.GreaterThanOrEqual);
        return (parameter, predicate);
    }

    public static (ParameterExpression Parameter, Expression Predicate) CreateLessThanOrEqualCondition<TProperty>(this Type type, string propertyName, TProperty value)
    {
        var parameter          = Expression.Parameter(type, "e");
        var propertyExpression = CreatePropertyExpression(parameter, propertyName);
        var predicate          = CreateComparisonExpression(propertyExpression, value, ExpressionType.LessThanOrEqual);
        return (parameter, predicate);
    }

    public static (ParameterExpression Parameter, Expression Predicate) CreateContainsCondition(this Type type, string propertyName, string? value)
    {
        var parameter          = Expression.Parameter(type, "e");
        var propertyExpression = CreatePropertyExpression(parameter, propertyName);

        if (string.IsNullOrWhiteSpace(value))
        {
            return (parameter, Expression.Constant(true));
        }

        if (propertyExpression.Type != typeof(string))
        {
            throw new ArgumentException("Contains predicate requires a string property.", nameof(propertyName));
        }

        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])
                             ?? throw new InvalidOperationException("Could not resolve string.Contains(string) method.");

        var propertyNotNull = Expression.NotEqual(propertyExpression, Expression.Constant(null, typeof(string)));
        var containsCheck   = Expression.Call(propertyExpression, containsMethod, Expression.Constant(value));
        var predicate       = Expression.AndAlso(propertyNotNull, containsCheck);

        return (parameter, predicate);
    }

    public static Expression AddContainsFilterCondition(this Expression predicate, ParameterExpression parameter, string propertyName, string? searchValue)
    {
        if (string.IsNullOrWhiteSpace(searchValue))
        {
            return predicate;
        }

        var property = CreatePropertyExpression(parameter, propertyName);
        if (property.Type != typeof(string))
        {
            return predicate;
        }

        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)]);

        if (containsMethod == null)
        {
            return predicate;
        }

        var containsCheck = Expression.Call(property, containsMethod, Expression.Constant(searchValue));
        return Expression.AndAlso(predicate, containsCheck);
    }

    public static (ParameterExpression? Parameter, Expression? Predicate) AddContainsFilterCondition<TEntity>(this Expression? predicate, ParameterExpression? parameter, string propertyName, string? searchValue)
    {
        if (string.IsNullOrWhiteSpace(searchValue))
        {
            return (parameter, predicate);
        }

        if (predicate is not null && parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter), "Parameter is required when predicate is provided.");
        }

        var workingParameter = parameter ?? Expression.Parameter(typeof(TEntity), "e");
        var workingPredicate = predicate ?? Expression.Constant(true);

        return (workingParameter, workingPredicate.AddContainsFilterCondition(workingParameter, propertyName, searchValue));
    }

    public static Expression AddEqualsFilterCondition(this Expression predicate, ParameterExpression parameter, string propertyName, object? searchValue)
    {
        return AddComparisonFilterCondition(predicate, parameter, propertyName, searchValue, ExpressionType.Equal);
    }

    public static (ParameterExpression? Parameter, Expression? Predicate) AddEqualsFilterCondition<TEntity>(this Expression? predicate, ParameterExpression? parameter, string propertyName, object? searchValue)
    {
        return AddComparisonFilterCondition<TEntity>(predicate, parameter, propertyName, searchValue, ExpressionType.Equal);
    }

    public static Expression AddGreaterThanOrEqualFilterCondition(this Expression predicate, ParameterExpression parameter, string propertyName, object? searchValue)
    {
        return AddComparisonFilterCondition(predicate, parameter, propertyName, searchValue, ExpressionType.GreaterThanOrEqual);
    }

    public static (ParameterExpression? Parameter, Expression? Predicate) AddGreaterThanOrEqualFilterCondition<TEntity>(this Expression? predicate, ParameterExpression? parameter, string propertyName, object? searchValue)
    {
        return AddComparisonFilterCondition<TEntity>(predicate, parameter, propertyName, searchValue, ExpressionType.GreaterThanOrEqual);
    }

    public static Expression AddLessThanOrEqualFilterCondition(this Expression predicate, ParameterExpression parameter, string propertyName, object? searchValue)
    {
        return AddComparisonFilterCondition(predicate, parameter, propertyName, searchValue, ExpressionType.LessThanOrEqual);
    }

    public static (ParameterExpression? Parameter, Expression? Predicate) AddLessThanOrEqualFilterCondition<TEntity>(this Expression? predicate, ParameterExpression? parameter, string propertyName, object? searchValue)
    {
        return AddComparisonFilterCondition<TEntity>(predicate, parameter, propertyName, searchValue, ExpressionType.LessThanOrEqual);
    }

    private static Expression AddComparisonFilterCondition(Expression predicate, ParameterExpression parameter, string propertyName, object? searchValue, ExpressionType comparisonType)
    {
        if (searchValue is null)
        {
            return predicate;
        }

        var property = CreatePropertyExpression(parameter, propertyName);

        if (!TryCreateComparisonExpression(property, searchValue, comparisonType, out var comparisonCheck))
        {
            return predicate;
        }

        return Expression.AndAlso(predicate, comparisonCheck!);
    }

    private static (ParameterExpression? Parameter, Expression? Predicate) AddComparisonFilterCondition<TEntity>(Expression? predicate, ParameterExpression? parameter, string propertyName, object? searchValue, ExpressionType comparisonType)
    {
        if (searchValue is null)
        {
            return (parameter, predicate);
        }

        if (predicate is not null && parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter), "Parameter is required when predicate is provided.");
        }

        var workingParameter = parameter ?? Expression.Parameter(typeof(TEntity), "e");
        var workingPredicate = predicate ?? Expression.Constant(true);

        return (workingParameter, AddComparisonFilterCondition(workingPredicate, workingParameter, propertyName, searchValue, comparisonType));
    }

    private static Expression CreateComparisonExpression(Expression property, object? value, ExpressionType comparisonType)
    {
        if (!TryCreateComparisonExpression(property, value, comparisonType, out var comparisonExpression))
        {
            throw new ArgumentException(
                $"Cannot create {comparisonType} expression for property type '{property.Type.Name}' with value '{value}'.",
                nameof(value));
        }

        return comparisonExpression!;
    }

    private static bool TryCreateComparisonExpression(Expression property, object? value, ExpressionType comparisonType, out Expression? comparisonExpression)
    {
        comparisonExpression = null;

        if (!TryBuildValueExpressionForProperty(property, value, out var valueExpression) || valueExpression is null)
        {
            return false;
        }

        comparisonExpression = comparisonType switch
        {
            ExpressionType.Equal              => Expression.Equal(property, valueExpression),
            ExpressionType.GreaterThanOrEqual => Expression.GreaterThanOrEqual(property, valueExpression),
            ExpressionType.LessThanOrEqual    => Expression.LessThanOrEqual(property, valueExpression),
            _                                 => throw new ArgumentOutOfRangeException(nameof(comparisonType), comparisonType, "Unsupported comparison type.")
        };

        return true;
    }

    private static bool TryBuildValueExpressionForProperty(Expression property, object? value, out Expression? valueExpression)
    {
        valueExpression = null;
        var comparisonType = Nullable.GetUnderlyingType(property.Type) ?? property.Type;

        if (value is null)
        {
            if (property.Type != comparisonType || !property.Type.IsValueType)
            {
                valueExpression = Expression.Constant(null, property.Type);
                return true;
            }

            return false;
        }

        if (!TryConvertValue(value, comparisonType, out var convertedValue))
        {
            return false;
        }

        var constant = Expression.Constant(convertedValue, comparisonType);
        valueExpression = property.Type != comparisonType
                              ? Expression.Convert(constant, property.Type)
                              : constant;
        return true;
    }

    private static Expression CreatePropertyExpression(Expression source, string propertyPath)
    {
        var members = propertyPath.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return members.Length == 0
                   ? throw new ArgumentException("Property path cannot be empty.", nameof(propertyPath))
                   : members.Aggregate(source, Expression.PropertyOrField);
    }

    private static bool TryConvertValue(object value, Type targetType, out object? convertedValue)
    {
        convertedValue = null;

        if (targetType.IsInstanceOfType(value))
        {
            convertedValue = value;
            return true;
        }

        try
        {
            if (targetType.IsEnum)
            {
                convertedValue = value is string enumText
                                     ? Enum.Parse(targetType, enumText, ignoreCase: true)
                                     : Enum.ToObject(targetType, value);
                return true;
            }

            if (targetType == typeof(Guid))
            {
                convertedValue = value is Guid guidValue ? guidValue : Guid.Parse(value.ToString() ?? string.Empty);
                return true;
            }

            convertedValue = Convert.ChangeType(value, targetType);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
