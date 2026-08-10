using System.Reflection;

namespace Lithan.Core.TestUtilities;

/// <summary>
/// Constructor Test Helper Methods
/// </summary>
public static class ConstructorTestHelper
{
    /// <summary>
    /// Helper method to construct any object and create default parameters as necessary
    /// </summary>
    /// <typeparam name="T">Object Type to be Constructed</typeparam>
    /// <param name="parameterName">Parameter Name of parameter that should contain null or specified value (Optional)</param>
    /// <param name="parameterValue">Parameter Value that should be used for specified parameter</param>
    /// <param name="allParametersMatch">All Parameter Names must match when looking for a matching constructor</param>
    /// <param name="constructorParams">Optional Constructor Parameters</param>
    /// <returns>Newly constructed object</returns>
    public static T? ConstructObject<T>(string? parameterName = null,
                                        object? parameterValue = null,
                                        bool allParametersMatch = false,
                                        params (string parameterName, object? parameterValue)[] constructorParams)
        where T : class
    {
        return ConstructObject(typeof(T), parameterName, parameterValue, allParametersMatch, constructorParams) as T;
    }

    /// <summary>
    /// Helper method to construct any object and create default parameters as necessary
    /// </summary>
    /// <param name="objectType">Object Type to be Constructed</param>
    /// <param name="parameterName">Parameter Name of parameter that should contain null or specified value (Optional)</param>
    /// <param name="parameterValue">Parameter Value that should be used for specified parameter</param>
    /// <param name="allParametersMatch">All Parameter Names must match when looking for a matching constructor</param>
    /// <param name="constructorParams">Optional Constructor Parameters</param>
    /// <returns>Newly constructed object</returns>
    public static object? ConstructObject(Type objectType,
                                          string? parameterName = null,
                                          object? parameterValue = null,
                                          bool allParametersMatch = false,
                                          params (string parameterName, object? parameterValue)[] constructorParams)
    {
        ConstructorInfo? constructorInfo;

        var allConstructors = objectType.GetConstructors().OrderByDescending(info => info.GetParameters().Length).ToList();
        if (allConstructors.Count == 1)
        {
            constructorInfo = allConstructors.First();
        }
        else
        {
            constructorInfo = allParametersMatch
                                  ? allConstructors.FirstOrDefault(info => info.GetParameters()
                                                                               .All(parameterInfo => constructorParams.ToList()
                                                                                                                      .Exists(tuple => tuple.parameterName == parameterInfo.Name)))
                                  : allConstructors.FirstOrDefault(info => info.GetParameters()
                                                                               .Any(parameterInfo => parameterInfo.Name == parameterName));

            if (constructorInfo == null)
            {
                constructorInfo = allConstructors.First();
            }
        }

        if (constructorInfo == null)
        {
            throw new Exception($"No Constructors found for object {objectType.FullName}");
        }

        var constructorParameters      = constructorInfo.GetParameters();
        var constructorParameterValues = new List<object?>();

        foreach (var currentParameter in constructorParameters)
        {
            if (!string.IsNullOrWhiteSpace(parameterName) && currentParameter.Name == parameterName)
            {
                constructorParameterValues.Add(parameterValue);
                continue;
            }

            if (constructorParams.Length > 0)
            {
                var (_, paramsValue) = constructorParams.FirstOrDefault(tuple => tuple.parameterName == currentParameter.Name);
                if (paramsValue != null)
                {
                    constructorParameterValues.Add(paramsValue);
                    continue;
                }
            }

            constructorParameterValues.Add(currentParameter.CreateRandomValue());
        }

        return constructorParameterValues.Any() 
                   ? constructorInfo.Invoke(constructorParameterValues.ToArray()) 
                   : Activator.CreateInstance(objectType);
    }
}