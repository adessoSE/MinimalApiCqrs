using MediatR;
using MinimalApiCqrs.Explicit.Core.Handler;

namespace MinimalApiCqrs.Explicit.Core.Metadata;

internal interface IOnSuccessMetadata { }

internal class OnSuccessMetadata<TRequest, TResponse> : IOnSuccessMetadata
    where TRequest : IRequest<TResponse>
{
    public SuccessHandlerWithResponseDelegate<TRequest, TResponse> Action { get; }

    public OnSuccessMetadata(SuccessHandlerWithResponseDelegate<TRequest, TResponse> action)
    {
        Action = action;
    }
}

internal class OnSuccessMetadata<TRequest> : IOnSuccessMetadata
    where TRequest : IRequest
{
    public SuccessHandlerDelegate<TRequest> Action { get; }

    public OnSuccessMetadata(SuccessHandlerDelegate<TRequest> action)
    {
        Action = action;
    }
}
