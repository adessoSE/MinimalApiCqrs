namespace MinimalApiCqrs.Simple.Core.CQRS;

using MediatR;

public interface IQuery<TResponse> : IRequest<TResponse>
    where TResponse : notnull { }
