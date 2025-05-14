namespace MinimalApiCqrs.Explicit.Core.Context;

public record ExceptionContext<TRequest, TException>(
    TRequest Request,
    TException Exception,
    ILogger Logger,
    string TraceIdentifier,
    HttpContext HttpContext
)
    where TException : Exception;
