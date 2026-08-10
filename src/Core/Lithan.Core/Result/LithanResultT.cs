using Lithan.Core.Models;

namespace Lithan.Core.Result;

/// <summary>
/// Lithan Result (Generic)
/// </summary>
/// <typeparam name="T">Result Values</typeparam>
public sealed class LithanResult<T>
{
    private readonly T? _resultValue;
    private readonly LithanError? _resultError;

    public LithanResult(T resultValue)
    {
        IsError      = false;
        _resultValue = resultValue;
        _resultError = null;
    }

    public LithanResult(LithanError resultError)
    {
        IsError      = true;
        _resultValue = default;
        _resultError = resultError;
    }

    /// <summary>
    /// Success indicator
    /// </summary>
    public bool IsError { get; }

    /// <summary>
    /// Error indicator
    /// </summary>
    public bool IsSuccess => !IsError;

    /// <summary>
    /// Result Values
    /// </summary>
    public T? Value => _resultValue;

    /// <summary>
    /// Result Error
    /// </summary>
    public LithanError? Error => _resultError;

    /// <summary>
    /// Success Implicit operator
    /// </summary>
    /// <param name="resultValue">Success Values</param>
    public static implicit operator LithanResult<T>(T resultValue) => new(resultValue);

    /// <summary>
    /// Error Implicit operator
    /// </summary>
    /// <param name="resultError">Error value</param>
    public static implicit operator LithanResult<T>(LithanError resultError) => new(resultError);

    /// <summary>
    /// Create Success Result
    /// </summary>
    /// <param name="successValue">Success Values</param>
    /// <returns>
    /// Newly created Success Result object
    /// </returns>
    public static LithanResult<T> Success(T successValue)
    {
        return new LithanResult<T>(successValue);
    }

    /// <summary>
    /// Create Failure Result
    /// </summary>
    /// <param name="failureValue">Failure value</param>
    /// <returns>
    /// Newly created Error Result object
    /// </returns>
    public static LithanResult<T> Failure(LithanError failureValue)
    {
        return new LithanResult<T>(failureValue);
    }

    /// <summary>
    /// Match Result to its appropriate result path
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="success">Success path</param>
    /// <param name="failure">Error path</param>
    /// <param name="nullValue">Null value path</param>
    /// <returns>
    /// Newly created result
    /// </returns>
    public TResult Match<TResult>(Func<T, TResult> success, Func<LithanError, TResult> failure, Func<TResult> nullValue)
    {
        return IsError switch
               {
                   false when _resultValue == null => nullValue(),
                   true                            => failure(_resultError!),
                   _                               => success(_resultValue)
               };
    }

    /// <summary>
    /// Match Result to its appropriate result path
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="success">Success path</param>
    /// <param name="failure">Error path</param>
    /// <returns>
    /// Newly created result
    /// </returns>
    public TResult Match<TResult>(Func<T, TResult> success, Func<LithanError, TResult> failure)
    {
        return IsError switch
               {
                   true => failure(_resultError!),
                   _    => success(_resultValue!)
               };
    }

    /// <summary>
    /// Match Result to its appropriate result path
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="success">Success path</param>
    /// <returns>
    /// Newly created result
    /// </returns>
    public TResult? Match<TResult>(Func<T, TResult> success)
    {
        return IsError switch
               {
                   true => default,
                   _    => success(_resultValue!)
               };
    }

    /// <summary>
    /// Match Result to its appropriate result path
    /// </summary>
    /// <param name="success">Success path</param>
    /// <param name="failure">Error path</param>
    /// <param name="nullValue">Null value path</param>
    public void Match(Action<T> success, Action<LithanError> failure, Action nullValue)
    {
        switch (IsError)
        {
            case false when _resultValue == null:
                nullValue.Invoke();
                return;
            case true:
                failure.Invoke(_resultError!);
                return;
            default:
                success(_resultValue);
                break;
        }
    }

    /// <summary>
    /// Match Result to its appropriate result path
    /// </summary>
    /// <param name="success">Success path</param>
    /// <param name="failure">Error path</param>
    public void Match(Action<T> success, Action<LithanError> failure)
    {
        switch (IsError)
        {
            case true:
                failure.Invoke(_resultError!);
                return;
            default:
                success(_resultValue!);
                break;
        }
    }

    /// <summary>
    /// Match Result to its appropriate result path
    /// </summary>
    /// <param name="success">Success path</param>
    public void Match(Action<T> success)
    {
        if (IsError)
        {
            return;
        }

        success(_resultValue!);
    }

    /// <summary>
    /// Match Result to its appropriate result path asynchronously
    /// </summary>
    /// <param name="success">Success path</param>
    /// <param name="failure">Error path</param>
    /// <param name="nullValue">Null value path</param>
    /// <returns>
    /// Newly created result
    /// </returns>
    public async Task<TResult> MatchAsync<TResult>(Func<T, Task<TResult>> success,
                                                   Func<LithanError, Task<TResult>> failure,
                                                   Func<Task<TResult>> nullValue)
    {
        return IsError switch
               {
                   false when _resultValue == null => await nullValue(),
                   true                            => await failure(_resultError!),
                   _                               => await success(_resultValue)
               };
    }

    /// <summary>
    /// Match Result to its appropriate result path asynchronously
    /// </summary>
    /// <param name="success">Success path</param>
    /// <param name="failure">Error path</param>
    /// <returns>
    /// Newly created result
    /// </returns>
    public async Task<TResult> MatchAsync<TResult>(Func<T, Task<TResult>> success,
                                                   Func<LithanError, Task<TResult>> failure)
    {
        return IsError switch
               {
                   true => await failure(_resultError!),
                   _    => await success(_resultValue!)
               };
    }

    /// <summary>
    /// Match Result to its appropriate result path asynchronously
    /// </summary>
    /// <param name="success">Success path</param>
    /// <returns>
    /// Newly created result
    /// </returns>
    public async Task<TResult?> MatchAsync<TResult>(Func<T, Task<TResult>> success)
    {
        return IsError switch
               {
                   true => default,
                   _    => await success(_resultValue!)
               };
    }

    /// <summary>
    /// Match Result to its appropriate result path asynchronously
    /// </summary>
    /// <param name="success">Success path</param>
    /// <param name="failure">Error path</param>
    /// <param name="nullValue">Null value path</param>
    public async Task MatchAsync(Func<T, Task> success, Func<LithanError, Task> failure, Func<Task> nullValue)
    {
        switch (IsError)
        {
            case false when _resultValue == null:
                await nullValue.Invoke();
                return;
            case true:
                await failure.Invoke(_resultError!);
                return;
            default:
                await success(_resultValue!);
                break;
        }
    }

    /// <summary>
    /// Match Result to its appropriate result path asynchronously
    /// </summary>
    /// <param name="success">Success path</param>
    /// <param name="failure">Error path</param>
    public async Task MatchAsync(Func<T, Task> success, Func<LithanError, Task> failure)
    {
        switch (IsError)
        {
            case true:
                await failure.Invoke(_resultError!);
                return;
            default:
                await success(_resultValue!);
                break;
        }
    }

    /// <summary>
    /// Match Result to its appropriate result path asynchronously
    /// </summary>
    /// <param name="success">Success path</param>
    public async Task MatchAsync(Func<T, Task> success)
    {
        if (IsError)
        {
            return;
        }

        await success(_resultValue!);
    }
}