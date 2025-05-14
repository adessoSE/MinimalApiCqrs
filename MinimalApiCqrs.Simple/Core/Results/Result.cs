namespace MinimalApiCqrs.Simple.Core.Results;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MinimalApiCqrs.Simple.Core.Results.Errors;

/// <summary>
/// Represents the result of an operation that may succeed or fail.
/// On failure, <see cref="Error"/> contains the failure details.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// When <c>false</c>, <see cref="Error"/> is guaranteed to be non-null.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }

    /// <summary>
    /// Indicates whether the operation failed.
    /// When <c>true</c>, <see cref="Error"/> is guaranteed to be non-null.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailure { get; }

    /// <summary>
    /// The <see cref="Error"/> describing why the operation failed;
    /// null if the operation succeeded.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="error">
    /// The <see cref="Error"/> instance describing failure, or null for success.
    /// </param>
    protected Result(Error? error)
    {
        IsSuccess = error is null;
        IsFailure = error is not null;
        Error = error;
    }

    /// <summary>
    /// Creates a successful <see cref="Result"/> without a value.
    /// </summary>
    /// <returns>A successful <see cref="Result"/> instance.</returns>
    public static Result Success() => new Result(null);

    /// <summary>
    /// Creates a successful <see cref="Result{TValue}"/> with the specified value.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    /// <param name="value">The value of the successful result.</param>
    /// <returns>A <see cref="Result{TValue}"/> carrying the value.</returns>
    public static Result<TValue> Success<TValue>(TValue value)
        where TValue : notnull => Result<TValue>.Success(value);

    /// <summary>
    /// Creates a failed <see cref="Result"/> with the given error code and message.
    /// </summary>
    /// <param name="code">The error code (kebab-case).</param>
    /// <param name="message">The error message (English, developer-facing).</param>
    /// <returns>A failed <see cref="Result"/> instance.</returns>
    public static Result Fail(string code, string message) => new Result(new Error(code, message));

    /// <summary>
    /// Creates a failed <see cref="Result"/> with the specified <see cref="Error"/>.
    /// </summary>
    /// <param name="error">The <see cref="Error"/> describing the failure.</param>
    /// <returns>A failed <see cref="Result"/> instance.</returns>
    public static Result Fail(Error error) => new Result(error);

    /// <summary>
    /// Creates a failed <see cref="Result"/> from the specified <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> to convert into an <see cref="Error"/>.</param>
    /// <returns>A failed <see cref="Result"/> instance representing the exception.</returns>
    public static Result Fail(Exception exception) =>
        new Result(ErrorExceptionMapper.ToError(exception));

    /// <summary>
    /// Creates a failed <typeparamref name="TResult"/> (or <see cref="Result"/>) from the given <see cref="Error"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of result to create.</typeparam>
    /// <param name="error">The <see cref="Error"/> describing the failure.</param>
    /// <returns>An instance of <typeparamref name="TResult"/> representing the failure.</returns>
    public static TResult Fail<TResult>(Error error)
        where TResult : Result
    {
        var resultType = typeof(TResult);

        if (resultType.IsGenericType)
        {
            var genericArgument = resultType.GetGenericArguments().First();

            var result = resultType
                .GetGenericTypeDefinition()
                .MakeGenericType(genericArgument)
                .GetMethod(nameof(Fail), [typeof(Error)])!
                .Invoke(null, [error])!;

            return (TResult)result;
        }

        return (TResult)Fail(error);
    }

    /// <summary>
    /// Creates a failed <typeparamref name="TResult"/> from the specified <see cref="Exception"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of result to create.</typeparam>
    /// <param name="exception">The <see cref="Exception"/> to convert into an <see cref="Error"/>.</param>
    /// <returns>An instance of <typeparamref name="TResult"/> representing the failure.</returns>
    public static TResult Fail<TResult>(Exception exception)
        where TResult : Result
    {
        var error = ErrorExceptionMapper.ToError(exception);

        return Fail<TResult>(error);
    }

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> to a failed <see cref="Result"/>.
    /// </summary>
    /// <param name="error">The <see cref="Error"/> to convert.</param>
    public static implicit operator Result(Error error) => Fail(error);

    /// <summary>
    /// Implicitly converts an <see cref="Exception"/> to a failed <see cref="Result"/>.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> to convert.</param>
    /// <returns>A failed <see cref="Result"/> corresponding to the exception.</returns>
    public static implicit operator Result(Exception exception) => Fail(exception);

#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
    /// <summary>
    /// Implicitly converts a <see cref="Result"/> to a completed <see cref="Task{Result}"/>.
    /// </summary>
    public static implicit operator Task<Result>(Result result) => Task.FromResult(result);
#pragma warning restore VSTHRD200

    /// <summary>
    /// Throws a mapped <see cref="Exception"/> if this result is a failure.
    /// Optionally logs the error before throwing.
    /// </summary>
    /// <param name="errorLogger">
    /// An <see cref="ILogger"/> to log the error; if null, no logging is performed.
    /// </param>
    /// <remarks>
    /// This method is hidden from stack traces and debuggers.
    /// </remarks>
    [DebuggerHidden]
    [StackTraceHidden]
    public void ThrowExceptionOnFailure(ILogger? errorLogger = null)
    {
        if (IsSuccess)
        {
            return;
        }

        if (errorLogger is not null)
        {
            Error.Log(errorLogger);
        }

        var exception = ErrorExceptionMapper.ToException(Error);

        ExceptionDispatchInfo.Capture(exception).Throw();
    }
}
