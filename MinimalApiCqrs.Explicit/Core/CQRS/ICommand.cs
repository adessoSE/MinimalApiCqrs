namespace MinimalApiCqrs.Simple.Core.CQRS;

using MediatR;

public interface ICommand : IRequest { }

public interface ICommand<TResult> : IRequest<TResult>
    where TResult : notnull { }
