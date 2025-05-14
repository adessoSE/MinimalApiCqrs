using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using MinimalApiCqrs.Explicit.Core.Context;

namespace MinimalApiCqrs.Explicit.Core.Handler;

public delegate TResult SuccessHandlerDelegate<TRequest, out TResult>(
    SuccessContext<TRequest> context
)
    where TRequest : IRequest
    where TResult : IResult;

public delegate IResult SuccessHandlerDelegate<TRequest>(SuccessContext<TRequest> context)
    where TRequest : IRequest;

public delegate TResult SuccessHandlerWithResponseDelegate<TRequest, TResponse, out TResult>(
    SuccessContext<TRequest, TResponse> context
)
    where TRequest : IRequest<TResponse>
    where TResult : IResult;

public delegate IResult SuccessHandlerWithResponseDelegate<TRequest, TResponse>(
    SuccessContext<TRequest, TResponse> context
)
    where TRequest : IRequest<TResponse>;

public static class SuccessHandler
{
    public static Ok<TResponse> Default<TRequest, TResponse>(
        SuccessContext<TRequest, TResponse> context
    ) => TypedResults.Ok(context.Response);

    public static NoContent Default<TRequest>(SuccessContext<TRequest> _) =>
        TypedResults.NoContent();
}
