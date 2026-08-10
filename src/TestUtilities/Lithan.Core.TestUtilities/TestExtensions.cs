using System.Reflection;
using NSubstitute;

namespace Lithan.Core.TestUtilities;

/// <summary>
/// Test Extension methods
/// </summary>
public static class TestExtensions
{
    /// <summary>
    /// Create Random Value for Parameter
    /// </summary>
    /// <param name="parameterInfo"></param>
    /// <returns></returns>
    public static object? CreateRandomValue(this ParameterInfo parameterInfo)
    {
        return RandomValueGenerator.CreateRandomValue(parameterInfo.ParameterType);
    }

    /// <summary>
    /// Create Random Value for Property
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <returns></returns>
    public static object? CreateRandomValue(this PropertyInfo propertyInfo)
    {
        return RandomValueGenerator.CreateRandomValue(propertyInfo.PropertyType);
    }

    /// <summary>
    /// Create a NSubstitute Mocked object
    /// </summary>
    /// <param name="objectType">Object Type</param>
    /// <returns>NSubstitute Mocked object</returns>
    public static object? CreateSubstitute<T>(this T objectType) where T : Type
    {
        if (objectType == typeof(Exception))
        {
            var exceptionMessage = RandomValueGenerator.CreateRandomString(10, 20);
            return new Exception(exceptionMessage);
        }

        var constructorInfo       = objectType.GetConstructors().MaxBy(info => info.GetParameters().Length);
        var constructorParameters = constructorInfo?.GetParameters();

        if (constructorInfo == null || constructorParameters == null || !constructorParameters.Any())
        {
            return Substitute.For(new Type[] { objectType }, Array.Empty<object>());
        }

        var parameterValues = TestHelper.CreateParameterValues(constructorParameters);
        if (objectType is { IsInterface: false, IsClass: true })
        {
            return Activator.CreateInstance(objectType, parameterValues.ToArray());
        }

        return Substitute.For(new Type[] { objectType }, parameterValues.ToArray());
    }
}