using System.Reflection;
using Xunit;
using AwesomeAssertions;

namespace Lithan.Core.TestUtilities.XUnit;

public static class XUnitAttributeTestHelper
{
    public static void ValidateMethodAttributes<T, TAttribute>(string methodName)
        where T : class
        where TAttribute : Attribute
    {
        var methodInfo = typeof(T).GetMethod(methodName);
        if (methodInfo == null)
        {
            throw new InvalidOperationException($"Method {methodName} not found in {typeof(T).Name}");
        }

        var attributes = methodInfo.GetCustomAttributes(typeof(TAttribute), false);
        attributes.Should().NotBeEmpty($"Expected method {methodName} to have attribute {typeof(TAttribute).Name}");
    }

    public static void ValidateMethodAttributes<T>(string methodName, Type expectedAttribute)
        where T : class
    {
        var methodInfo = typeof(T).GetMethod(methodName);
        if (methodInfo == null)
        {
            throw new InvalidOperationException($"Method {methodName} not found in {typeof(T).Name}");
        }

        var attributes = methodInfo.GetCustomAttributes(expectedAttribute, false);
        attributes.Should().NotBeEmpty($"Expected method {methodName} to have attribute {expectedAttribute.Name}");
    }

    public static void ValidateMethodAttributes<T>(string methodName, Type expectedAttribute,
                                                   (string propertyName, object expectedValue) valueTuple)
    {
        var methodInfo = typeof(T).GetMethod(methodName);
        if (methodInfo == null)
        {
            throw new InvalidOperationException($"Method {methodName} not found in {typeof(T).Name}");
        }

        var attributes = methodInfo.GetCustomAttributes(expectedAttribute).ToList();
        if (attributes == null || !attributes.Any())
        {
            throw new InvalidOperationException($"Attribute {expectedAttribute.Name} not found on method {methodName}");
        }

        foreach (var currentAttribute in attributes)
        {
            var propertyInfo = expectedAttribute.GetProperty(valueTuple.propertyName);
            if (propertyInfo == null)
            {
                throw new InvalidOperationException($"Property {valueTuple.propertyName} not found on attribute {expectedAttribute.Name}");
            }

            var actualValue = propertyInfo.GetValue(currentAttribute);
            if (actualValue != null && actualValue.Equals(valueTuple.expectedValue))
            {
                return;
            }
        }

        Assert.Fail($"Expected property {valueTuple.propertyName} to be {valueTuple.expectedValue}, but was no such value found");
    }
}