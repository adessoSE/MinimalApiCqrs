namespace MinimalApiCqrs.Explicit.Core.Exceptions;

/// <summary>
/// A base exception class that allows to filter for specific business exceptions that occurred.
/// <para>
/// This exception should only be used for business error descriptions. If the exception is technical and/or not relevant for the consuming user, use <see cref="ApplicationException"/> instead.
/// </para>
/// </summary>
public class BusinessException : ApplicationException
{
    public BusinessExceptionSeverity Severity { get; }
    public string Solution { get; }
    public string Title => this.Message;

    public BusinessException(BusinessExceptionSeverity severity, string title, string solution)
        : base(title)
    {
        Severity = severity;
        Solution = solution;
    }

    public BusinessException(
        BusinessExceptionSeverity severity,
        string title,
        string solution,
        Exception innerException
    )
        : base(title, innerException)
    {
        Severity = severity;
        Solution = solution;
    }
}
