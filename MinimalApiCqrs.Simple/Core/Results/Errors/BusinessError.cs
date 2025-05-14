namespace MinimalApiCqrs.Simple.Core.Results.Errors;

using Microsoft.Extensions.Logging;

/// <summary>
/// Levels for classifying the impact of a <see cref="BusinessError"/>.
/// </summary>
public enum BusinessErrorSeverity
{
    /// <summary>The issue is purely informational (no user action required).</summary>
    Info = 0,

    /// <summary>The issue is a warning; user may need to adjust input or retry.</summary>
    Warn = 1,

    /// <summary>The issue is an error; user action is required to continue.</summary>
    Error = 2,
}

/// <summary>
/// Categories for <see cref="BusinessError"/>s to drive HTTP status and UI flow.
/// </summary>
public enum BusinessErrorType
{
    /// <summary>400-level validation or malformed request.</summary>
    BadRequest = 0,

    /// <summary>409-level state conflict.</summary>
    Conflict = 1,

    /// <summary>500-level unanticipated business rule failure.</summary>
    Unexpected = 2,

    /// <summary>404 Not Found – referenced resource missing.</summary>
    NotFound = 3,
}

/// <summary>
/// An end-user–facing error.
/// Messages must be localized; an optional <see cref="Solution"/> can guide self-remediation.
/// </summary>
/// <param name="Code">Machine-readable error code (kebab-case).</param>
/// <param name="Message">Localized user message.</param>
/// <param name="Solution">Optional next-step suggestion.</param>
/// <param name="Severity">Impact level for UI treatment.</param>
/// <param name="ErrorType">Classification of this business error.</param>
public record BusinessError(
    string Code,
    string Message,
    string? Solution = null,
    BusinessErrorSeverity Severity = BusinessErrorSeverity.Error,
    BusinessErrorType ErrorType = BusinessErrorType.BadRequest
) : Error(Code, Message)
{
    /// <summary>
    /// 400 Bad Request – client sent invalid data.
    /// </summary>
    public static BusinessError BadRequest(string code, string message) =>
        new BusinessError(code, message, ErrorType: BusinessErrorType.BadRequest);

    /// <summary>
    /// 409 Conflict – action violates current state or constraints.
    /// </summary>
    public static BusinessError Conflict(string code, string message) =>
        new BusinessError(code, message, ErrorType: BusinessErrorType.Conflict);

    /// <summary>
    /// Unexpected business logic failure; code indicates specific scenario.
    /// </summary>
    public static BusinessError Unexpected(string code, string message) =>
        new BusinessError(code, message, ErrorType: BusinessErrorType.Unexpected);

    /// <summary>
    /// No-op: business errors are user-visible, not dev-logged.
    /// </summary>
    /// <param name="logger">Ignored.</param>
    public override void Log(ILogger logger)
    {
        // Intentionally blank: we don’t log user-facing errors.
    }
}

/// <summary>
/// Exception wrapper for a <see cref="BusinessError"/> in user-facing flows.
/// </summary>
public class BusinessException : ErrorException<BusinessError>
{
    /// <summary>
    /// Initializes a new <see cref="BusinessException"/> from a <see cref="BusinessError"/>.
    /// </summary>
    public BusinessException(BusinessError error)
        : base(error) { }
}
