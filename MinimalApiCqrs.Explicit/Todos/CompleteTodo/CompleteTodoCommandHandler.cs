namespace MinimalApiCqrs.Simple.Todos.CompleteTodo;

using System.Threading;
using System.Threading.Tasks;
using MinimalApiCqrs.Explicit.Core.Exceptions;
using MinimalApiCqrs.Simple.Core.CQRS;
using MinimalApiCqrs.Simple.Todos;

internal class CompleteTodoCommandHandler : ICommandHandler<CompleteTodoCommand>
{
    private readonly ITodoRepository todoRepository;

    public CompleteTodoCommandHandler(ITodoRepository todoRepository)
    {
        this.todoRepository = todoRepository;
    }

    public async Task Handle(CompleteTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await this.todoRepository.GetTodoByIdAsync(request.TodoId, cancellationToken);

        if (todo is null)
        {
            throw new NotFoundException(request.TodoId.ToString());
        }

        if (todo.IsCompleted)
        {
            throw new BusinessException(
                BusinessExceptionSeverity.Error,
                $"Todo {todo.Id} is already completed.",
                string.Empty
            );
        }

        todo.Complete();

        await this.todoRepository.UpdateTodoAsync(todo, cancellationToken);
    }
}
