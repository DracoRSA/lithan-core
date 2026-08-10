using Lithan.Core.Abstractions;

namespace Lithan.Core.Models;

/// <summary>
/// Lithan Error
/// </summary>
public sealed class LithanError : ILithanError
{
    public LithanError(int errorCode, string message, Exception? exception = null)
    {
        if (errorCode <= 0) { throw new ArgumentOutOfRangeException(nameof(errorCode)); }

        ErrorCode = errorCode;
        Message   = message ?? throw new ArgumentNullException(nameof(message));
        Exception = exception;
    }

    /// <inheritdoc />
    public int ErrorCode { get; }

    /// <inheritdoc />
    public string Message { get; }

    /// <inheritdoc />
    public Exception? Exception { get; }

    /// <summary>
    /// Create a String representation of the error
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Error: {ErrorCode} - {Message}{(Exception == null ? "" : $"\n{Exception}")}";
}