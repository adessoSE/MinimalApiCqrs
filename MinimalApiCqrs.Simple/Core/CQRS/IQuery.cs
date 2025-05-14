namespace MinimalApiCqrs.Simple.Core.CQRS;

using MediatR;
using MinimalApiCqrs.Simple.Core.Results;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    where TResponse : notnull { }
