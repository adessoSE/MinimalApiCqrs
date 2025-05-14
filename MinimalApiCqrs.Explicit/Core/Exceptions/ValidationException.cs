namespace MinimalApiCqrs.Explicit.Core.Exceptions;

public record ValidationExceptionError(string Property, string[] Errors);

public class ValidationException : BusinessException
{
    public ValidationExceptionError[] ValidationErrors { get; }

    public ValidationException(IEnumerable<ValidationExceptionError> errors)
        : base(
            BusinessExceptionSeverity.Error,
            "Validierung fehlgeschlagen",
            "Bitte beheben Sie die Validierungsfehler"
        )
    {
        this.ValidationErrors = errors.ToArray();
    }

    public ValidationException(params ValidationExceptionError[] errors)
        : base(
            BusinessExceptionSeverity.Error,
            "Validierung fehlgeschlagen",
            "Bitte beheben Sie die Validierungsfehler"
        )
    {
        this.ValidationErrors = errors.ToArray();
    }
}
