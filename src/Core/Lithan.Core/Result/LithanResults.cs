using Lithan.Core.Models;

namespace Lithan.Core.Result;

/// <summary>
/// Lithan Result
/// </summary>
/// <typeparam name="T">Result Values</typeparam>
public sealed class LithanResults<T>
{
    private readonly List<T>? _resultValues;
    private readonly LithanError? _resultError;

    public LithanResults(List<T>? resultValues)
    {
        IsError       = false;
        _resultValues = resultValues;
        _resultError  = null;
    }

    public LithanResults(LithanError resultError)
    {
        IsError       = true;
        _resultValues = null;
        _resultError  = resultError;
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
    public List<T>? Values => _resultValues;

    /// <summary>
    /// Result Error
    /// </summary>
    public LithanError? Error => _resultError;

    /// <summary>
    /// Success Implicit operator
    /// </summary>
    /// <param name="resultValues">Success Values</param>
    public static implicit operator LithanResults<T>(List<T> resultValues) => new(resultValues);

    /// <summary>
    /// Error Implicit operator
    /// </summary>
    /// <param name="resultError">Error value</param>
    public static implicit operator LithanResults<T>(LithanError resultError) => new(resultError);

    /// <summary>
    /// Create Success Result
    /// </summary>
    /// <param name="successValues">Success Values</param>
    /// <returns>
    /// Newly created Success Result object
    /// </returns>
    public static LithanResults<T> Success(List<T>? successValues)
    {
        return new LithanResults<T>(successValues);
    }

    /// <summary>
    /// Create Failure Result
    /// </summary>
    /// <param name="failureValue">Failure value</param>
    /// <returns>
    /// Newly created Error Result object
    /// </returns>
    public static LithanResults<T> Failure(LithanError failureValue)
    {
        return new LithanResults<T>(failureValue);
    }

    /// <summary>
    /// Match Result to its appropriate result path
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="success">Success path</param>
    /// <returns>
    /// Newly created result
    /// </returns>
    public TResult? Match<TResult>(Func<List<T>, TResult> success)
    {
        return IsError switch
               {
                   true => default,
                   _    => success(_resultValues!)
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
    public TResult Match<TResult>(Func<List<T>, TResult> success, Func<LithanError, TResult> failure)
    {
        return IsError switch
               {
                   true => failure(_resultError!),
                   _    => success(_resultValues!)
               };
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
    public TResult? Match<TResult>(Func<List<T>, TResult> success, Func<LithanError, TResult> failure, Func<TResult> nullValue)
    {
        return IsError switch
               {
                   false when _resultValues == null => nullValue(),
                   true                             => failure(_resultError!),
                   _                                => success(_resultValues)
               };
    }

    /// <summary>
    /// Match Result to its appropriate result path
    /// </summary>
    /// <param name="success">Success path</param>
    public void Match(Action<List<T>> success)
    {
        if (IsError)
        {
            return;
        }

        success(_resultValues!);
    }

    /// <summary>
    /// Match Result to its appropriate result path
    /// </summary>
    /// <param name="success">Success path</param>
    /// <param name="failure">Error path</param>
    public void Match(Action<List<T>> success, Action<LithanError> failure)
    {
        switch (IsError)
        {
            case true:
                failure.Invoke(_resultError!);
                return;
            default:
                success(_resultValues!);
                break;
        }
    }

    /// <summary>
    /// Match Result to its appropriate result path
    /// </summary>
    /// <param name="success">Success path</param>
    /// <param name="failure">Error path</param>
    /// <param name="nullValue">Null value path</param>
    public void Match(Action<List<T>> success, Action<LithanError> failure, Action nullValue)
    {
        switch (IsError)
        {
            case false when _resultValues == null:
                nullValue.Invoke();
                return;
            case true:
                failure.Invoke(_resultError!);
                return;
            default:
                success(_resultValues);
                break;
        }
    }

    /// <summary>
    /// Match Result to its appropriate result path asynchronously
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="success">Success path</param>
    /// <returns></returns>
    public async Task<TResult?> MatchAsync<TResult>(Func<List<T>, Task<TResult>> success)
    {
        return IsError switch
               {
                   true => default,
                   _    => await success(_resultValues!)
               };
    }

    /// <summary>
    /// Match Result to its appropriate result path asynchronously
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="success">Success path</param>
    /// <param name="failure">Error</param>
    /// <returns></returns>
    public async Task<TResult> MatchAsync<TResult>(Func<List<T>, Task<TResult>> success, Func<LithanError, Task<TResult>> failure)
    {
        return IsError switch
               {
                   true => await failure.Invoke(_resultError!),
                   _    => await success(_resultValues!)
               };
    }

    /// <summary>
    /// Match Result to its appropriate result path asynchronously
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="success">Success path</param>
    /// <param name="failure">Error</param>
    /// <param name="nullValue">Null value(s) path</param>
    /// <returns></returns>
    public async Task<TResult?> MatchAsync<TResult>(Func<List<T>, Task<TResult>> success, Func<LithanError, Task<TResult>> failure, Func<Task<TResult>> nullValue)
    {
        return IsError switch
               {
                   false when _resultValues == null => await nullValue.Invoke(),
                   true                             => await failure.Invoke(_resultError!),
                   _                                => await success(_resultValues)
               };
    }

    /// <summary>
    /// Match Result to its appropriate result path asynchronously
    /// </summary>
    /// <param name="success">Success path</param>
    public async Task MatchAsync(Func<List<T>, Task> success)
    {
        if (IsError)
        {
            return;
        }

        await success(_resultValues!);
    }

    /// <summary>
    /// Match Result to its appropriate result path asynchronously
    /// </summary>
    /// <param name="success">Success path</param>
    /// <param name="failure">Error</param>
    public async Task MatchAsync(Func<List<T>, Task> success, Func<LithanError, Task> failure)
    {
        switch (IsError)
        {
            case true:
                await failure.Invoke(_resultError!);
                return;
            default:
                await success(_resultValues!);
                break;
        }
    }

    /// <summary>
    /// Match Result to its appropriate result path asynchronously
    /// </summary>
    /// <param name="success">Success path</param>
    /// <param name="failure">Error</param>
    /// <param name="nullValue">Null value(s) path</param>
    public async Task MatchAsync(Func<List<T>, Task> success, Func<LithanError, Task> failure, Func<Task> nullValue)
    {
        switch (IsError)
        {
            case false when _resultValues == null:
                await nullValue.Invoke();
                return;
            case true:
                await failure.Invoke(_resultError!);
                return;
            default:
                await success(_resultValues);
                break;
        }
    }
}