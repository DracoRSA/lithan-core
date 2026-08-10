namespace Lithan.Core.Abstractions;

/// <summary>
/// Lithan Error
/// </summary>
public interface ILithanError
{
    /// <summary>
    /// Error Code
    /// </summary>
    int ErrorCode { get; }

    /// <summary>
    /// Error Message
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Exception
    /// </summary>
    Exception? Exception { get; }
}