namespace MinimalApiCqrs.Simple.Core.Results.Errors;

using System.Collections.Immutable;
using Microsoft.Extensions.Logging;

/// <summary>
/// End-user–facing validation failure.
/// Contains one or more <see cref="ValidationResult"/> entries to pinpoint fields or objects that need correction.
/// </summary>
/// <param name="ValidationResults">
/// Collection of individual validation results, each mapping a source (field or object) to error messages.
/// </param>
public sealed record ValidationError(
    ImmutableArray<ValidationError.ValidationResult> ValidationResults
) : Error(Codes.Validation, "Validation error")
{
    /// <summary>
    /// Represents validation errors for a single source (field or object).
    /// </summary>
    /// <param name="Source">Identifier of the field or object that failed validation.</param>
    /// <param name="ErrorMessages">List of user-facing messages explaining what went wrong.</param>
    public sealed record ValidationResult(string Source, ImmutableArray<string> ErrorMessages);

    /// <summary>
    /// No-op: validation errors are presented to users, not logged for developers.
    /// </summary>
    /// <param name="logger">Ignored.</param>
    public override void Log(ILogger logger)
    {
        // Intentionally left blank.
    }
}

/// <summary>
/// Exception wrapper for a <see cref="ValidationError"/> in user-facing validation flows.
/// </summary>
public class ValidationErrorException : ErrorException<ValidationError>
{
    /// <summary>
    /// Initializes a new <see cref="ValidationErrorException"/> from a <see cref="ValidationError"/> instance.
    /// </summary>
    /// <param name="error">The <see cref="ValidationError"/> to wrap.</param>
    public ValidationErrorException(ValidationError error)
        : base(error) { }
}
