namespace MinimalApiCqrs.Simple.Core.Results;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MinimalApiCqrs.Simple.Core.Results.Errors;

/// <summary>
/// Represents the result of an operation that may succeed with a value of type <typeparamref name="TValue"/>
/// or fail with an <see cref="Error"/>.
/// </summary>
/// <typeparam name="TValue">The type of the success value; must be non-nullable.</typeparam>
public class Result<TValue> : Result
    where TValue : notnull
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// When <c>true</c>, <see cref="Value"/> is non-null and <see cref="Error"/> is null.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(true, nameof(Value))]
    public new bool IsSuccess => base.IsSuccess;

    /// <summary>
    /// Indicates whether the operation failed.
    /// When <c>true</c>, <see cref="Error"/> is non-null and <see cref="Value"/> is null.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Error))]
    [MemberNotNullWhen(false, nameof(Value))]
    public new bool IsFailure => base.IsFailure;

    /// <inheritdoc/>
    public new Error? Error => base.Error;

    /// <summary>
    /// The successful value of type <typeparamref name="TValue"/>,
    /// or null if the result represents a failure.
    /// </summary>
    public TValue? Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class.
    /// </summary>
    /// <param name="value">The success value, or default if failure.</param>
    /// <param name="error">The <see cref="Error"/> describing failure, or null for success.</param>
    protected Result(TValue? value, Error? error)
        : base(error)
    {
        Value = value;
    }

    /// <summary>
    /// Deconstructs the result into <paramref name="value"/> and <paramref name="error"/>.
    /// </summary>
    /// <param name="value">When successful, the value; otherwise default.</param>
    /// <param name="error">When failure, the <see cref="Error"/>; otherwise null.</param>
    public void Deconstruct(out TValue? value, out Error? error)
    {
        value = Value;
        error = Error;
    }

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
    [MemberNotNull(nameof(Value))]
    public new void ThrowExceptionOnFailure(ILogger? errorLogger = null)
    {
        base.ThrowExceptionOnFailure(errorLogger);

        if (IsFailure)
        {
            throw new InvalidOperationException(
                $"Impossible, but satisfy compiler null check for value."
            );
        }
    }

    /// <summary>
    /// Creates a successful <see cref="Result{TValue}"/> with the specified value.
    /// </summary>
    /// <param name="value">The success value.</param>
    /// <returns>A successful <see cref="Result{TValue}"/> instance.</returns>
    public static Result<TValue> Success(TValue value) => new Result<TValue>(value, null);

    /// <summary>
    /// Creates a failed <see cref="Result{TValue}"/> with the given error code and message.
    /// </summary>
    /// <param name="code">The error code (kebab-case).</param>
    /// <param name="message">The error message (English, developer-facing).</param>
    /// <returns>A failed <see cref="Result{TValue}"/> instance.</returns>
    public static new Result<TValue> Fail(string code, string message) =>
        new Result<TValue>(default, new Error(code, message));

    /// <summary>
    /// Creates a failed <see cref="Result{TValue}"/> with the specified <see cref="Error"/>.
    /// </summary>
    /// <param name="error">The <see cref="Error"/> describing the failure.</param>
    /// <returns>A failed <see cref="Result{TValue}"/> instance.</returns>
    public static new Result<TValue> Fail(Error error) => new Result<TValue>(default, error);

    /// <summary>
    /// Creates a failed <see cref="Result{TValue}"/> from the specified <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> to convert into an <see cref="Error"/>.</param>
    /// <returns>A failed <see cref="Result{TValue}"/> instance.</returns>
    public static new Result<TValue> Fail(Exception exception) =>
        new Result<TValue>(default, ErrorExceptionMapper.ToError(exception));

    /// <summary>
    /// Implicitly converts a <typeparamref name="TValue"/> to a successful <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <returns>A successful <see cref="Result{TValue}"/> instance containing the value.</returns>
    public static implicit operator Result<TValue>(TValue value) => Success(value);

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> to a failed <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="error">The <see cref="Error"/> to convert.</param>
    /// <returns>A failed <see cref="Result{TValue}"/> instance representing the error.</returns>
    public static implicit operator Result<TValue>(Error error) => Fail(error);

    /// <summary>
    /// Implicitly converts an <see cref="Exception"/> to a failed <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> to convert.</param>
    /// <returns>A failed <see cref="Result{TValue}"/> instance representing the exception.</returns>
    public static implicit operator Result<TValue>(Exception exception) => Fail(exception);

#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
    /// <summary>
    /// Implicitly converts a <see cref="Result{TValue}"/> to a completed <see cref="Task{Result{TValue}}"/>.
    /// </summary>
    public static implicit operator Task<Result<TValue>>(Result<TValue> result) =>
        Task.FromResult(result);
#pragma warning restore VSTHRD200
}
