namespace MinimalApiCqrs.Simple.Core.CQRS;

using MediatR;
using MinimalApiCqrs.Simple.Core.Results;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
    new Task<Result> Handle(TCommand command, CancellationToken cancellationToken);
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
}

public interface ICommandHandler<in TCommand, TResponse>
    : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
    where TResponse : notnull
{
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
    new Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken);
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
}
