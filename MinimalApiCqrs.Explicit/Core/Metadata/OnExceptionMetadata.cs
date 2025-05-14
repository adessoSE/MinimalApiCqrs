using MinimalApiCqrs.Explicit.Core.Context;
using MinimalApiCqrs.Explicit.Core.Handler;

namespace MinimalApiCqrs.Explicit.Core.Metadata;

internal interface IOnExceptionMetadata
{
    public Type ExceptionType { get; }

    IResult? MapExceptionToResult(object request, Exception exception, HttpContext httpContext);
}

internal class OnExceptionMetadata<TRequest, TException> : IOnExceptionMetadata
    where TException : Exception
{
    public ExceptionHandlerDelegate<TRequest, TException> Action { get; }

    public Type ExceptionType { get; } = typeof(TException);

    public OnExceptionMetadata(ExceptionHandlerDelegate<TRequest, TException> action)
    {
        Action = action;
    }

    public IResult? MapExceptionToResult(
        object request,
        Exception exception,
        HttpContext httpContext
    )
    {
        var typedException = exception as TException;
        var typedRequest = (TRequest)request;

        if (typedException is null)
        {
            return null;
        }

        var logger = httpContext.RequestServices.GetRequiredService<
            ILogger<SuccessContext<TRequest>>
        >();
        var exceptionContext = new ExceptionContext<TRequest, TException>(
            typedRequest,
            typedException,
            logger,
            httpContext.TraceIdentifier,
            httpContext
        );

        return Action(exceptionContext);
    }
}
