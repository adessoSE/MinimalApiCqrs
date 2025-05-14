namespace MinimalApiCqrs.Simple.Core.CQRS;

using MediatR;
using MinimalApiCqrs.Simple.Core.Results;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
    new Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
}
