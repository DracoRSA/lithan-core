using System.Reflection;
using Xunit;
using AwesomeAssertions;

namespace Lithan.Core.TestUtilities.XUnit;

public static class XUnitMethodTestHelper
{
    /// <summary>
    /// Validate that if a specified argument value is null, that a ArgumentNullException is thrown on an Async method
    /// </summary>
    /// <typeparam name="T">Type under test</typeparam>
    /// <param name="methodName">Method to be tested</param>
    /// <param name="parameterName">Method Parameter Name to verify</param>
    /// <param name="parameterValue">Parameter Value (Default null)</param>
    public static async Task ValidateArgumentNullExceptionIfParameterIsNullAsync<T>(string methodName, string parameterName, object? parameterValue = null)
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
            if (!string.IsNullOrWhiteSpace(parameterName) && currentParameter.Name == parameterName)
            {
                methodParameterValues.Add(parameterValue!);
                continue;
            }

            methodParameterValues.Add(currentParameter.CreateRandomValue()!);
        }

        var constructedObject = ConstructorTestHelper.ConstructObject(typeof(T));
        try
        {
            var methodTask = (Task)methodInfo.Invoke(constructedObject, methodParameterValues.ToArray())!;
            await methodTask;
            Assert.Fail($"Argument Null Exception not throw for Method [{methodName}] Parameter [{parameterName}] on {typeof(T).FullName}");
        }
        catch (ArgumentNullException argumentNullException)
        {
            argumentNullException.ParamName.Should().Be(parameterName);
        }
        catch (TargetInvocationException exception) when (exception.InnerException is ArgumentNullException argumentNullException)
        {
            argumentNullException.ParamName.Should().Be(parameterName);
        }
    }

    /// <summary>
    /// Validate that if a specified argument value is null, that a ArgumentNullException is thrown on a synchronous method
    /// </summary>
    /// <typeparam name="T">Type under test</typeparam>
    /// <param name="methodName">Method to be tested</param>
    /// <param name="parameterName">Method Parameter Name to verify</param>
    /// <param name="parameterValue">Parameter Value (Default null)</param>
    public static void ValidateArgumentNullExceptionIfParameterIsNull<T>(string methodName, string parameterName, object? parameterValue = null)
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
            if (!string.IsNullOrWhiteSpace(parameterName) && currentParameter.Name == parameterName)
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
        catch (ArgumentNullException argumentNullException)
        {
            argumentNullException.ParamName.Should().Be(parameterName);
        }
        catch (TargetInvocationException ex) when (ex.InnerException is ArgumentNullException argumentNullException)
        {
            argumentNullException.ParamName.Should().Be(parameterName);
        }
    }

    private static MethodInfo GetMethodInformation<T>(string methodName, string parameterName)
    {
        var allMethodInfos = typeof(T).GetMethods().Where(info => info.Name == methodName).ToList();
        if (!allMethodInfos.Any())
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

        if (methodInfo == null)
        {
            throw new InvalidOperationException($"Method [{methodName}] does not contain parameter named {parameterName}");
        }

        return methodInfo;
    }
}
