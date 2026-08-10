using Lithan.Core.Models;

namespace Lithan.Core.Result;

/// <summary>
/// Lithan Result
/// </summary>
public sealed class LithanResult
{
    private readonly LithanError? _resultError;

    public LithanResult()
    {
        IsError      = false;
        _resultError = default;
    }

    public LithanResult(LithanError resultError)
    {
        IsError      = true;
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
    /// Result Error
    /// </summary>
    public LithanError? Error => _resultError;

    /// <summary>
    /// Error Implicit operator
    /// </summary>
    /// <param name="resultError">Error value</param>
    public static implicit operator LithanResult(LithanError resultError) => new(resultError);

    /// <summary>
    /// Create Success Result
    /// </summary>
    /// <returns>
    /// Newly created Success Result object
    /// </returns>
    public static LithanResult Success()
    {
        return new LithanResult();
    }

    /// <summary>
    /// Create Failure Result
    /// </summary>
    /// <param name="failureValue">Failure value</param>
    /// <returns>
    /// Newly created Error Result object
    /// </returns>
    public static LithanResult Failure(LithanError failureValue)
    {
        return new LithanResult(failureValue);
    }

    /// <summary>
    /// Match Result to its appropriate result path
    /// </summary>
    /// <param name="success">Success path</param>
    /// <param name="failure">Error path</param>
    public void Match(Action success, Action<LithanError> failure)
    {
        switch (IsError)
        {
            case true:
                failure.Invoke(_resultError!);
                return;
            default:
                success();
                break;
        }
    }

    /// <summary>
    /// Match Result to its appropriate result path
    /// </summary>
    /// <param name="success">Success path</param>
    public void Match(Action success)
    {
        if (IsError)
        {
            return;
        }

        success();
    }

    /// <summary>
    /// Match Result to its appropriate result path
    /// </summary>
    /// <param name="success">Success path</param>
    /// <param name="failure">Error path</param>
    public TResult Match<TResult>(Func<TResult> success, Func<LithanError, TResult> failure)
    {
        return IsError switch
        {
            true => failure(_resultError!),
            _ => success()
        };
    }

    /// <summary>
    /// Match Result to its appropriate result path
    /// </summary>
    /// <param name="success">Success path</param>
    public TResult? Match<TResult>(Func<TResult> success)
    {
        return IsError ? default : success();
    }

    /// <summary>
    /// Match Result to its appropriate result path
    /// </summary>
    /// <param name="success">Success path</param>
    /// <param name="failure">Error path</param>
    public async Task MatchAsync(Func<Task> success, Func<LithanError, Task> failure)
    {
        switch (IsError)
        {
            case true:
                await failure.Invoke(_resultError!);
                return;
            default:
                await success();
                break;
        }
    }

    /// <summary>
    /// Match Result to its appropriate result path
    /// </summary>
    /// <param name="success">Success path</param>
    public async Task MatchAsync(Func<Task> success)
    {
        if (IsError)
        {
            return;
        }
        await success();
    }

    /// <summary>
    /// Match Result to its appropriate result path
    /// </summary>
    /// <param name="success">Success path</param>
    /// <param name="failure">Error path</param>
    public async Task<TResult> MatchAsync<TResult>(Func<Task<TResult>> success, Func<LithanError, Task<TResult>> failure)
    {
        switch (IsError)
        {
            case true:
                return await failure.Invoke(_resultError!);
            default:
                return await success();
        }
    }

    /// <summary>
    /// Match Result to its appropriate result path
    /// </summary>
    /// <param name="success">Success path</param>
    public async Task<TResult?> MatchAsync<TResult>(Func<Task<TResult>> success)
    {
        if (IsError)
        {
            return default;
        }

        return await success();
    }
}