using MediatR;
using MinimalApiCqrs.Explicit.Core.Handler;

namespace MinimalApiCqrs.Explicit.Core.Exceptions;

internal class NoExceptionHandlerException : Exception
{
    public NoExceptionHandlerException(string routeName)
        : base(
            $"No exception handler found for {routeName}. Configure the exception handler with .{nameof(ExplicitRouteHandlerBuilder<IRequest>.OnException)} while your route building. "
                + $"You can use {nameof(ExceptionHandler)}.{nameof(ExceptionHandler.Default)} for the default handling."
        ) { }
}
