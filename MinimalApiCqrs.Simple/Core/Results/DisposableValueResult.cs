namespace MinimalApiCqrs.Simple.Core.Results;

using System;
using System.Threading.Tasks;
using MinimalApiCqrs.Simple.Core.Results.Errors;

/// <summary>
/// A disposable variant of <see cref="Result{TValue}"/>,
/// where <typeparamref name="TValue"/> implements <see cref="IDisposable"/>.
/// </summary>
/// <typeparam name="TValue">The disposable value type.</typeparam>
public class DisposableResult<TValue> : Result<TValue>, IDisposable
    where TValue : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableResult{TValue}"/> class.
    /// </summary>
    /// <param name="value">The disposable value, or default if failure.</param>
    /// <param name="error">The <see cref="Error"/> describing failure, or null for success.</param>
    protected DisposableResult(TValue? value, Error? error)
        : base(value, error) { }

    /// <summary>
    /// Creates a successful <see cref="DisposableResult{TValue}"/> with the specified value.
    /// </summary>
    /// <param name="value">The disposable value.</param>
    /// <returns>A successful <see cref="DisposableResult{TValue}"/> instance.
    /// </returns>
    public static new DisposableResult<TValue> Success(TValue value) =>
        new DisposableResult<TValue>(value, null);

    /// <summary>
    /// Creates a failed <see cref="DisposableResult{TValue}"/> with the given error code and message.
    /// </summary>
    /// <param name="code">The error code (kebab-case).</param>
    /// <param name="message">The error message (English, developer-facing).</param>
    /// <returns>A failed <see cref="DisposableResult{TValue}"/> instance.</returns>
    public static new DisposableResult<TValue> Fail(string code, string message) =>
        new DisposableResult<TValue>(default, new Error(code, message));

    /// <summary>
    /// Creates a failed <see cref="DisposableResult{TValue}"/> with the specified <see cref="Error"/>.
    /// </summary>
    /// <param name="error">The <see cref="Error"/> describing the failure.</param>
    /// <returns>A failed <see cref="DisposableResult{TValue}"/> instance.</returns>
    public static new DisposableResult<TValue> Fail(Error error) =>
        new DisposableResult<TValue>(default, error);

    public static new DisposableResult<TValue> Fail(Exception exception) =>
        new DisposableResult<TValue>(default, ErrorExceptionMapper.ToError(exception));

    /// <summary>
    /// Implicitly converts a <typeparamref name="TValue"/> to a successful <see cref="DisposableResult{TValue}"/>.
    /// </summary>
    /// <param name="value">The disposable value to wrap.</param>
    public static implicit operator DisposableResult<TValue>(TValue value) => Success(value);

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> to a failed <see cref="DisposableResult{TValue}"/>.
    /// </summary>
    /// <param name="error">The <see cref="Error"/> to convert.</param>
    public static implicit operator DisposableResult<TValue>(Error error) => Fail(error);

    public static implicit operator DisposableResult<TValue>(Exception exception) =>
        Fail(exception);

#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
    /// <summary>
    /// Implicitly converts a <see cref="DisposableResult{TValue}"/> to a completed <see cref="Task{DisposableResult{TValue}}"/>.
    /// </summary>
    public static implicit operator Task<DisposableResult<TValue>>(
        DisposableResult<TValue> result
    ) => Task.FromResult(result);
#pragma warning restore VSTHRD200

    /// <summary>
    /// Disposes the underlying value if the result was successful.
    /// </summary>
    public void Dispose()
    {
        Value?.Dispose();
    }
}
