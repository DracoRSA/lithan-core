namespace Lithan.Core.TestUtilities.UnitTests.Fakes;

public class FakeException : Exception
{
    public FakeException(int errorCode, string exceptionMessage)
        : this(errorCode, exceptionMessage, null)
    {
    }

    public FakeException(int errorCode, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    public int ErrorCode { get; }
}