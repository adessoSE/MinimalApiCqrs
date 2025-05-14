namespace MinimalApiCqrs.Explicit.Core.Exceptions;

/// <summary>
/// A Base exception class that allows to filter for specific exception that occurred in the application context.
/// <para>
/// This exception should only be used for technical error descriptions. If the exception is relevant for the consuming user, use <see cref="BusinessException"/> instead.
/// </para>
/// </summary>
public class ApplicationException : Exception
{
    public ApplicationException(string message)
        : base(message) { }

    public ApplicationException(string message, Exception innerException)
        : base(message, innerException) { }
}
