namespace MinimalApiCqrs.Explicit.Core.Context;

public record SuccessContext<TRequest>(TRequest Request, ILogger Logger, HttpContext HttpContext);

public record SuccessContext<TRequest, TResponse>(
    TRequest Request,
    TResponse Response,
    ILogger Logger,
    HttpContext HttpContext
) : SuccessContext<TRequest>(Request, Logger, HttpContext);
