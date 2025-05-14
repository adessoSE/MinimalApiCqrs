namespace MinimalApiCqrs.Simple.Core.Results.Errors;

using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using MinimalApiCqrs.Simple.Core.Results;

/// <summary>
/// Internal-only error type; carries developer diagnostics and must not be shown to end users.
/// </summary>
[JsonPolymorphic(
    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor
)]
[JsonDerivedType(typeof(BusinessError), typeDiscriminator: "business")]
[JsonDerivedType(typeof(ValidationError), typeDiscriminator: "validation")]
public record Error
{
    public string Code { get; }
    public string Message { get; }
    public string? TraceId { get; }
    public DateTimeOffset OccurredAt { get; }

    public Error(string code, string message)
    {
        Code = code;
        Message = message;
        TraceId = Activity.Current?.TraceId.ToString();
        OccurredAt = DateTimeOffset.UtcNow;
    }

#pragma warning disable VSTHRD200 // no-Async suffix on implicit conversion
    /// <summary>
    /// Allows returning an <see cref="Error"/> directly as a failed <see cref="Result"/> in async flows.
    /// </summary>
    public static implicit operator Task<Result>(Error error) =>
        Task.FromResult(Result.Fail(error));
#pragma warning restore VSTHRD200

    /// <summary>
    /// Kebab-case error codes used throughout the system.
    /// </summary>
    public static class Codes
    {
        public const string NotFound = "not-found";
        public const string Forbidden = "forbidden";
        public const string Validation = "validation";
        public const string Unexpected = "unexpected";
        public const string TaskCanceled = "task-canceled";
    }

    /// <summary>
    /// Create a <see cref="NotFoundError"/> with a default "resource not found" message.
    /// </summary>
    public static NotFoundError NotFound(string? message = null) =>
        NotFound(Codes.NotFound, message);

    /// <summary>
    /// Create a <see cref="NotFoundError"/> with a custom code and optional message.
    /// </summary>
    public static NotFoundError NotFound(string code, string? message = null) =>
        new NotFoundError(message ?? "The requested resource was not found", code);

    /// <summary>
    /// A singleton "forbidden" error (HTTP 403).
    /// </summary>
    public static Error Forbidden() => new Error(Codes.Forbidden, "Forbidden access");

    /// <summary>
    /// Alias for <see cref="Forbidden"/> (treat unauthorized as forbidden).
    /// </summary>
    public static Error Unauthorized() => Forbidden();

    /// <summary>
    /// Wrap any <see cref="Exception"/> into an <see cref="UnexpectedError"/>.
    /// </summary>
    public static UnexpectedError Unexpected(Exception exception) => new UnexpectedError(exception);

    /// <summary>
    /// Converts an internal error into its public‐facing equivalent.
    /// Unrecognized errors fall back to a generic business error.
    /// </summary>
    public Error ToPublicError() =>
        this switch
        {
            { Code: Codes.Forbidden } => Forbidden(),
            NotFoundError => this,
            { Code: Codes.NotFound } => NotFound(Message),
            BusinessError => this,
            ValidationError => this,
            TaskCanceledError => this,
            _ => BusinessError.Unexpected(Codes.Unexpected, "An unexpected error occurred"),
        };

    /// <summary>
    /// Logs the error at Error level, using English messages and structured properties.
    /// </summary>
    /// <param name="logger">Your application logger.</param>
    public virtual void Log(ILogger logger)
    {
        logger.LogError("{ErrorCode} - {ErrorMessage}", Code, Message);
    }
}

/// <summary>
/// Exception wrapper that carries an <see cref="Error"/> instance.
/// </summary>
public class ErrorException : Exception
{
    /// <summary>
    /// The underlying <see cref="Error"/> that caused this exception.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Initialize from an <see cref="Error"/>; exception message is set to <c>Error.Message</c>.
    /// </summary>
    public ErrorException(Error error)
        : base(error.Message)
    {
        Error = error;
    }
}

/// <summary>
/// Generic exception wrapper for a specific <see cref="Error"/> subtype.
/// </summary>
/// <typeparam name="TError">Any type derived from <see cref="Error"/>.</typeparam>
public class ErrorException<TError> : ErrorException
    where TError : Error
{
    /// <summary>
    /// The specific <typeparamref name="TError"/> instance.
    /// </summary>
    public new TError Error { get; }

    /// <summary>
    /// Initialize from a <typeparamref name="TError"/>; base constructor handles message.
    /// </summary>
    public ErrorException(TError error)
        : base(error)
    {
        Error = error;
    }
}

/// <summary>
/// Thrown to represent a forbidden‐access error (user‐visible).
/// </summary>
public class ForbiddenException : ErrorException
{
    /// <summary>
    /// Creates a <see cref="ForbiddenException"/> using the standard <see cref="Error.Forbidden"/>.
    /// </summary>
    public ForbiddenException()
        : base(Error.Forbidden()) { }
}
