namespace MinimalApiCqrs.Simple.Core.Results.Errors;

/// <summary>
/// User-visible "not found" error with customizable code and optional message.
/// Defaults to "The requested resource was not found" if no message is provided.
/// </summary>
/// <param name="Message">Optional localized user message.</param>
/// <param name="Code">Machine-readable error code (kebab-case).</param>
public record NotFoundError(string? Message = null, string Code = Error.Codes.NotFound)
    : Error(Code, Message ?? "The requested resource was not found") { }

/// <summary>
/// Exception thrown for a <see cref="NotFoundError"/> in user-facing flows.
/// </summary>
public class NotFoundException : ErrorException<NotFoundError>
{
    /// <summary>
    /// Wraps the given <see cref="NotFoundError"/> in an exception.
    /// </summary>
    /// <param name="error">The <see cref="NotFoundError"/> instance.</param>
    public NotFoundException(NotFoundError error)
        : base(error) { }
}
