namespace MinimalApiCqrs.Simple.Core.Results.Errors;

using System;
using Microsoft.Extensions.Logging;

/// <summary>
/// Internal-only error representing a canceled operation.
/// Carries the original <see cref="Exception"/>; not exposed to end users.
/// </summary>
/// <param name="Exception">
/// The underlying cancellation exception whose message is used as the error message.
/// </param>
public sealed record TaskCanceledError(Exception Exception)
    : Error(Codes.TaskCanceled, Exception.Message)
{
    /// <summary>
    /// Logs a warning with the original exception and structured error details.
    /// </summary>
    /// <param name="logger">The application logger.</param>
    public override void Log(ILogger logger)
    {
        logger.LogWarning(Exception, "{ErrorCode} - {ErrorMessage}", Code, Message);
    }
}

/// <summary>
/// Exception wrapper for an internal <see cref="TaskCanceledError"/>.
/// </summary>
public class TaskCanceledErrorException : ErrorException
{
    /// <summary>
    /// Initializes a new <see cref="TaskCanceledErrorException"/> from the given cancellation exception.
    /// </summary>
    /// <param name="inner">
    /// The exception that caused the task cancellation.
    /// </param>
    public TaskCanceledErrorException(Exception inner)
        : base(new TaskCanceledError(inner)) { }
}
