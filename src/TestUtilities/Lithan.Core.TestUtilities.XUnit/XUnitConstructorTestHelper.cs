using System.Reflection;
using Xunit;
using AwesomeAssertions;
using Thuria.Zitidar.Extensions;

namespace Lithan.Core.TestUtilities.XUnit;

/// <summary>
/// XUnit Constructor Test Helper
/// </summary>
public static class XUnitConstructorTestHelper
{
    /// <summary>
    /// Validate that when a null parameter is given to a constructor, an ArgumentNullException is thrown
    /// </summary>
    /// <typeparam name="T">Object Type to test</typeparam>
    /// <param name="parameterName">Parameter Name to test</param>
    /// <param name="constructorParams">Optional Constructor Parameters</param>
    public static void ValidateArgumentNullExceptionIfParameterIsNull<T>(string parameterName,
                                                                         params (string parameterName, object? parameterValue)[] constructorParams)
        where T : class
    {
        try
        {
            ConstructorTestHelper.ConstructObject<T>(parameterName, constructorParams: constructorParams);

            Assert.Fail($"ArgumentNullException not throw for Constructor Parameter [{parameterName}] on {typeof(T).FullName}");
        }
        catch (TargetInvocationException invocationException)
        {
            var argumentNullException = invocationException.InnerException as ArgumentNullException;
            if (argumentNullException == null)
            {
                Assert.Fail($"ArgumentNullException not throw for Constructor Parameter [{parameterName}] on {typeof(T).FullName}");
            }

            argumentNullException.ParamName.Should().Be(parameterName);
        }
    }

    /// <summary>
    /// Validate that the specified Exception is thrown if the specified parameter value is null
    /// </summary>
    /// <typeparam name="T">Object to test</typeparam>
    /// <typeparam name="TException">Exception expected to be thrown</typeparam>
    /// <param name="parameterName">Parameter Name to test</param>
    /// <param name="allParametersMatch">All Parameter Names must match when looking for a matching constructor</param>
    /// <param name="constructorParams">Optional Constructor Parameters</param>
    public static void ValidateExceptionIsThrownIfParameterIsNull<T, TException>(string parameterName,
                                                                                 bool allParametersMatch = false,
                                                                                 params (string parameterName, object? ParameterValue)[] constructorParams)
        where T : class
        where TException : Exception
    {
        try
        {
            ConstructorTestHelper.ConstructObject<T>(parameterName, allParametersMatch: allParametersMatch, constructorParams: constructorParams);
            Assert.Fail($"{typeof(TException).Name} Exception not throw for Constructor Parameter [{parameterName}] on {typeof(T).FullName}");
        }
        catch (TargetInvocationException invocationException)
        {
            var thrownException = invocationException.InnerException as TException;
            if (thrownException == null)
            {
                Assert.Fail($"{typeof(TException).Name} Exception not throw for Constructor Parameter [{parameterName}] on {typeof(T).FullName}");
            }

            thrownException.Message.Should().Contain(parameterName);
        }
    }

    /// <summary>
    /// Validate that a property is set with the value given during the constructing of the object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameterName">Constructor parameter Name</param>
    /// <param name="propertyName">Object Property Name</param>
    /// <param name="allParametersMatch">All Parameter Names must match when looking for a matching constructor</param>
    /// <param name="constructorParams">Optional Constructor Parameters</param>
    public static void ValidatePropertySetWithParameter<T>(string parameterName,
                                                           string propertyName,
                                                           bool allParametersMatch = false,
                                                           params (string parameterName, object parameterValue)[] constructorParams)
        where T : class
    {
        var parameterValue = typeof(T).GetProperty(propertyName)?.CreateRandomValue();
        var parameterList = Enumerable.Range(0, constructorParams.Length)
                                      .ToDictionary(i => constructorParams[i].parameterValue, i => constructorParams[i].parameterValue);

        parameterList[parameterName] = parameterValue!;

        var objectUnderTest = ConstructorTestHelper.ConstructObject<T>(parameterName,
                                                                       parameterValue,
                                                                       allParametersMatch,
                                                                       parameterList.Select(pair => (pair.Key.ToString()!, pair.Value ?? null)).ToArray());
        if (objectUnderTest == null)
        {
            Assert.Fail($"Failed to create {typeof(T).FullName} to test Property Get and Set for {propertyName}");
        }

        var propertyValue = objectUnderTest.GetPropertyValue(propertyName);
        propertyValue.Should().Be(parameterValue, $"because parameter [{parameterName}] of the constructor of [{typeof(T).FullName}] should set property [{propertyName}]");
    }
}