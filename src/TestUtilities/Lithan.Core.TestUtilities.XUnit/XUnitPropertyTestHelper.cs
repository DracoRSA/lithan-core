using System.Text;
using System.Reflection;
using Xunit;
using AwesomeAssertions;
using Thuria.Zitidar.Extensions;

namespace Lithan.Core.TestUtilities.XUnit;

/// <summary>
/// XUnit Property Test Helper
/// </summary>
public static class XUnitPropertyTestHelper
{
    /// <summary>
    /// Validate when setting a property, the value is actually available via get
    /// </summary>
    /// <param name="objectUnderTest"></param>
    /// <param name="propertyName"></param>
    public static void ValidateGetAndSet(this object objectUnderTest, string propertyName)
    {
        if (propertyName == null) { throw new ArgumentNullException(nameof(propertyName)); }

        var propertyInfo = objectUnderTest.GetType().GetProperty(propertyName);
        if (propertyInfo == null)
        {
            throw new InvalidOperationException($"Property [{propertyName}] does not exists on {objectUnderTest.GetType().FullName}");
        }

        var propertyValue = propertyInfo.CreateRandomValue();

        objectUnderTest.SetPropertyValue(propertyName, propertyValue!);
        var returnedValue = objectUnderTest.GetPropertyValue(propertyName);

        returnedValue.Should().BeEquivalentTo(propertyValue);
    }

    /// <summary>
    /// Validate when setting a property, the value is actually available via get
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyName"></param>
    public static void ValidateGetAndSet<T>(string propertyName)
        where T : class
    {
        var objectUnderTest = ConstructorTestHelper.ConstructObject<T>();
        if (objectUnderTest == null)
        {
            Assert.Fail($"Failed to create {typeof(T).FullName} to test Property Get and Set for {propertyName}");
        }

        objectUnderTest.ValidateGetAndSet(propertyName);
    }

    /// <summary>
    /// Validate that a property has been decorated with a specified Attribute
    /// </summary>
    /// <typeparam name="T">Object Type under test</typeparam>
    /// <param name="propertyName">Object Property Name</param>
    /// <param name="attributeType">Attribute Type</param>
    /// <param name="attributePropertyValues">Attribute Property Values</param>
    public static void ValidateDecoratedWithAttribute<T>(string propertyName, Type attributeType,
                                                         List<(string propertyName, object propertyValue)>? attributePropertyValues = null)
        where T : class
    {
        if (propertyName == null)
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        if (attributeType == null)
        {
            throw new ArgumentNullException(nameof(attributeType));
        }

        var objectUnderTest = ConstructorTestHelper.ConstructObject<T>();
        if (objectUnderTest == null)
        {
            Assert.Fail($"Failed to create {typeof(T).FullName} to test Property {propertyName} decorated with {attributeType.Name}");
        }

        var propertyInfo = objectUnderTest.GetType().GetProperty(propertyName);
        if (propertyInfo == null)
        {
            Assert.Fail($"Property [{propertyName}] does not exists on {objectUnderTest.GetType().FullName}");
        }

        var customAttribute = propertyInfo.GetCustomAttribute(attributeType);
        if (customAttribute == null)
        {
            Assert.Fail($"Property {propertyName} is not decorated with {attributeType.Name} Attribute");
        }

        var errorMessage = new StringBuilder();
        if (attributePropertyValues != null)
        {
            foreach (var (attributePropertyName, attributePropertyValue) in attributePropertyValues)
            {
                var attributePropertyInfo = attributeType.GetProperty(attributePropertyName);
                if (attributePropertyInfo == null)
                {
                    errorMessage.AppendLine($"{propertyName} Property is decorated with {attributeType.Name} " +
                                            $"but the attribute property {attributePropertyName} does not exist on the attribute");
                    continue;
                }

                var propertyValue = attributePropertyInfo.GetValue(customAttribute);
                if (propertyValue == null || !propertyValue.Equals(attributePropertyValue))
                {
                    errorMessage.AppendLine($"{propertyName} Property is decorated with {attributeType.Name} " +
                                            $"but the attribute property {attributePropertyName} is not set to {attributePropertyValue}");
                }
            }
        }

        if (errorMessage.Length > 0)
        {
            Assert.Fail(errorMessage.ToString());
        }
    }
}