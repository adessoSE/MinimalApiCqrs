namespace MinimalApiCqrs.Simple.Core.CQRS;

using MediatR;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand
{
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
    new Task Handle(TCommand command, CancellationToken cancellationToken);
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
}

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : notnull
{
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
    new Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken);
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
}
