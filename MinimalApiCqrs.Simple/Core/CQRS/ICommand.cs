namespace MinimalApiCqrs.Simple.Core.CQRS;

using MediatR;
using MinimalApiCqrs.Simple.Core.Results;

public interface ICommand : IRequest<Result> { }

public interface ICommand<TResult> : IRequest<Result<TResult>>
    where TResult : notnull { }
