using System.Reflection;
using Xunit;
using AwesomeAssertions;

namespace Lithan.Core.TestUtilities;

/// <summary>
/// Method Test Helper Extension methods
/// </summary>
public static class MethodTestHelper
{
    /// <summary>
    /// Validate that if a specified argument value is null, that a ArgumentNullException is thrown on a method
    /// </summary>
    /// <typeparam name="T">Type under test</typeparam>
    /// <param name="methodName">Method to be tested</param>
    /// <param name="parameterName">Method Parameter Name to verify</param>
    /// <param name="parameterValue">Parameter Value (Default null)</param>
    public static void ValidateArgumentNullExceptionIsThrownIfParameterIsNull<T>(string methodName, string parameterName, object? parameterValue = null)
    {
        if (methodName == null)
        {
            throw new ArgumentNullException(nameof(methodName));
        }

        if (parameterName == null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        var methodInfo            = GetMethodInformation<T>(methodName, parameterName);
        var methodParameters      = methodInfo.GetParameters();
        var methodParameterValues = new List<object>();

        foreach (var currentParameter in methodParameters)
        {
            if (!string.IsNullOrWhiteSpace(parameterName) &&
                currentParameter.Name == parameterName)
            {

                methodParameterValues.Add(parameterValue!);
                continue;
            }

            methodParameterValues.Add(currentParameter.CreateRandomValue()!);
        }

        var constructedObject = ConstructorTestHelper.ConstructObject(typeof(T));

        try
        {
            methodInfo.Invoke(constructedObject, methodParameterValues.ToArray());
            Assert.Fail($"Argument Null Exception not throw for Method [{methodName}] Parameter [{parameterName}] on {typeof(T).FullName}");
        }
        catch (ArgumentNullException exception)
        {
            exception.ParamName.Should().Be(parameterName);
            Assert.True(true);
        }
        catch (TargetInvocationException exception)
        {
            exception.InnerException.Should().NotBeNull();
            exception.InnerException.Should().BeOfType<ArgumentNullException>();
            ((ArgumentNullException)exception.InnerException!).ParamName.Should().Be(parameterName);
            Assert.True(true);
        }
    }

    /// <summary>
    /// Validate that if a specified argument value is null, that a specified exception is thrown on a method
    /// </summary>
    /// <typeparam name="T">Type under test</typeparam>
    /// <typeparam name="TException">Exception expected to be thrown</typeparam>
    /// <param name="methodName">Method to be tested</param>
    /// <param name="parameterName">Method Parameter Name to verify</param>
    /// <param name="parameterValue">Parameter Value (Default null)</param>
    public static void ValidateExceptionIsThrownIfParameterIsNull<T, TException>(string methodName, string parameterName, object? parameterValue = null)
        where TException : Exception, new()
    {
        if (methodName == null)
        {
            throw new ArgumentNullException(nameof(methodName));
        }

        if (parameterName == null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        var methodInfo            = GetMethodInformation<T>(methodName, parameterName);
        var methodParameters      = methodInfo.GetParameters();
        var methodParameterValues = new List<object>();

        foreach (var currentParameter in methodParameters)
        {
            if (!string.IsNullOrWhiteSpace(parameterName) &&
                currentParameter.Name == parameterName)
            {
                methodParameterValues.Add(parameterValue!);
                continue;
            }

            methodParameterValues.Add(currentParameter.CreateRandomValue()!);
        }

        var constructedObject = ConstructorTestHelper.ConstructObject(typeof(T));

        try
        {
            methodInfo.Invoke(constructedObject, methodParameterValues.ToArray());
            Assert.Fail($"{typeof(TException).Namespace} Exception not throw for Method [{methodName}] Parameter [{parameterName}] on {typeof(T).FullName}");
        }
        catch (TargetInvocationException exception)
        {
            if (exception.InnerException?.GetType() != typeof(TException))
            {
                Assert.Fail($"{typeof(TException).Namespace} Exception not throw for Method [{methodName}] Parameter [{parameterName}] on {typeof(T).FullName}");
            }

            Assert.True(true);
        }
        catch (Exception exception)
        {
            if (exception.GetType() != typeof(TException))
            {
                Assert.Fail($"{typeof(TException).Namespace} Exception not throw for Method [{methodName}] Parameter [{parameterName}] on {typeof(T).FullName}");
            }

            Assert.True(true);
        }
    }

    /// <summary>
    /// Validate that if a specified argument value is null, that a ArgumentNullException is thrown on an Async method
    /// </summary>
    /// <typeparam name="T">Type under test</typeparam>
    /// <param name="methodName">Method to be tested</param>
    /// <param name="parameterName">Method Parameter Name to verify</param>
    /// <param name="parameterValue">Parameter Value (Default null)</param>
    public static async Task ValidateArgumentNullExceptionIsThrownIfParameterIsNullAsync<T>(string methodName, string parameterName, object? parameterValue = null)
    {
        if (methodName == null)
        {
            throw new ArgumentNullException(nameof(methodName));
        }

        if (parameterName == null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        var methodInfo            = GetMethodInformation<T>(methodName, parameterName);
        var methodParameters      = methodInfo.GetParameters();
        var methodParameterValues = new List<object>();

        foreach (var currentParameter in methodParameters)
        {
            if (!string.IsNullOrWhiteSpace(parameterName) &&
                currentParameter.Name == parameterName)
            {

                methodParameterValues.Add(parameterValue!);
                continue;
            }

            methodParameterValues.Add(currentParameter.CreateRandomValue()!);
        }

        var constructedObject = ConstructorTestHelper.ConstructObject(typeof(T));

        try
        {
            var methodTask = (Task)methodInfo.Invoke(constructedObject!, methodParameterValues.ToArray())!;
            if (methodTask != null)
            {
                await methodTask;
                Assert.Fail($"Argument Null Exception not throw for Method [{methodName}] Parameter [{parameterName}] on {typeof(T).FullName}");
            }
        }
        catch (ArgumentNullException argumentNullException)
        {
            argumentNullException.ParamName.Should().Be(parameterName);
            Assert.True(true);
        }
        catch (TargetInvocationException exception) when (exception.InnerException is ArgumentNullException argumentNullException)
        {
            argumentNullException.ParamName.Should().Be(parameterName);
            Assert.True(true);
        }
    }

    /// <summary>
    /// Validate that if a specified argument value is null, that a specified exception is thrown on an Async method
    /// </summary>
    /// <typeparam name="T">Type under test</typeparam>
    /// <typeparam name="TException">Exception expected to be thrown</typeparam>
    /// <param name="methodName">Method to be tested</param>
    /// <param name="parameterName">Method Parameter Name to verify</param>
    /// <param name="parameterValue">Parameter Value (Default null)</param>
    public static async Task ValidateExceptionIsThrownIfParameterIsNullAsync<T, TException>(string methodName, string parameterName, object? parameterValue = null)
        where TException : Exception, new()
    {
        if (methodName == null)
        {
            throw new ArgumentNullException(nameof(methodName));
        }

        if (parameterName == null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        var methodInfo            = GetMethodInformation<T>(methodName, parameterName);
        var methodParameters      = methodInfo.GetParameters();
        var methodParameterValues = new List<object>();

        foreach (var currentParameter in methodParameters)
        {
            if (!string.IsNullOrWhiteSpace(parameterName) &&
                currentParameter.Name == parameterName)
            {
                methodParameterValues.Add(parameterValue!);
                continue;
            }

            methodParameterValues.Add(currentParameter.CreateRandomValue()!);
        }

        var constructedObject = ConstructorTestHelper.ConstructObject(typeof(T));

        try
        {
            var methodTask = (Task)methodInfo.Invoke(constructedObject!, methodParameterValues.ToArray())!;
            if (methodTask != null)
            {
                await methodTask;
                Assert.Fail($"{typeof(TException).Namespace} Exception not throw for Method [{methodName}] Parameter [{parameterName}] on {typeof(T).FullName}");
            }
        }
        catch (TException exception)
        {
            exception.Message.Should().Contain(parameterName);
            Assert.True(true);
        }
        catch (TargetInvocationException exception) when (exception.InnerException is TException typedException)
        {
            typedException.Message.Should().Contain(parameterName);
            Assert.True(true);
        }
    }

    private static MethodInfo GetMethodInformation<T>(string methodName, string parameterName)
    {
        var allMethodInfos = typeof(T).GetMethods().Where(info => info.Name.Equals(methodName)).ToList();
        if (allMethodInfos == null || !allMethodInfos.Any())
        {
            Assert.Fail($"Method [{methodName}] does not exists on {typeof(T).FullName}");
        }

        MethodInfo? methodInfo = null;
        if (allMethodInfos.Count > 1)
        {
            foreach (var currentMethodInfo in allMethodInfos)
            {
                var parameterInfo = currentMethodInfo.GetParameters().FirstOrDefault(info => info.Name == parameterName);
                if (parameterInfo == null)
                {
                    continue;
                }

                methodInfo = currentMethodInfo;
                break;
            }
        }
        else
        {
            methodInfo = allMethodInfos.First();
        }

        return methodInfo!;
    }
}