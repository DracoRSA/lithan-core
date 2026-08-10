using Xunit;

namespace Lithan.Core.TestUtilities.XUnit;

/// <summary>
/// XUnit Assert Extensions
/// </summary>
public static class XUnitAssertExtensions
{
    /// <summary>
    /// Does not throw the specified Exception
    /// </summary>
    /// <param name="assert">Assert</param>
    /// <param name="testAction">Test Action</param>
    /// <param name="errorMessage">Optional Error Message</param>
    public static void DoesNotThrow(this Assert assert, Action testAction, string? errorMessage = null)
    {
        try
        {
            testAction();
        }
        catch (Exception runtimeException)
        {
            Assert.Fail(errorMessage ?? $"Expected not to throw Exception, but Exception was thrown\n{runtimeException}");
        }
    }

    /// <summary>
    /// Does not throw the specified Exception
    /// </summary>
    /// <typeparam name="T">Exception Type</typeparam>
    /// <param name="assert">Assert</param>
    /// <param name="testAction">Test Action</param>
    /// <param name="errorMessage">Optional Error Message</param>
    public static void DoesNotThrow<T>(this Assert assert, Action testAction, string? errorMessage = null)
        where T : Exception
    {
        try
        {
            testAction();
        }
        catch (T runtimeException)
        {
            Assert.Fail(errorMessage ?? $"Expected not to throw {typeof(T)} Exception, but {nameof(T)} Exception was thrown\n{runtimeException}");
        }
    }
}