using MediatR;
using MinimalApiCqrs.Explicit.Core.Handler;

namespace MinimalApiCqrs.Explicit.Core.Exceptions;

internal class NoSuccessHandlerException : Exception
{
    public NoSuccessHandlerException(string routeName)
        : base(
            $"No success handler found for {routeName}. Configure the success handler with .{nameof(ExplicitRouteHandlerBuilder<IRequest>.OnSuccess)} while your route building. "
                + $"You can use {nameof(SuccessHandler)}.{nameof(SuccessHandler.Default)} for the default handling."
        ) { }
}
