namespace MinimalApiCqrs.Simple.Core.Results.Errors;

using System;
using Microsoft.Extensions.Logging;

/// <summary>
/// Internal-only error for unhandled exceptions.
/// Wraps the original <see cref="Exception"/> to preserve stack trace; not exposed to end users.
/// </summary>
/// <param name="Exception">
/// The exception that triggered this unexpected error.
/// </param>
public sealed record UnexpectedError(Exception Exception)
    : Error(Codes.Unexpected, "An unexpected error occurred")
{
    /// <summary>
    /// Logs the exception and error details at Error level, retaining the original stack trace.
    /// </summary>
    /// <param name="logger">The application logger.</param>
    public override void Log(ILogger logger)
    {
        logger.LogError(Exception, "{ErrorCode} - {ErrorMessage}", Code, Message);
    }
}

/// <summary>
/// Exception wrapper for an internal <see cref="UnexpectedError"/>.
/// </summary>
public class UnexpectedErrorException : ErrorException
{
    /// <summary>
    /// Initializes a new <see cref="UnexpectedErrorException"/> wrapping the given exception.
    /// </summary>
    /// <param name="inner">The original exception to preserve.</param>
    public UnexpectedErrorException(Exception inner)
        : base(new UnexpectedError(inner)) { }
}
