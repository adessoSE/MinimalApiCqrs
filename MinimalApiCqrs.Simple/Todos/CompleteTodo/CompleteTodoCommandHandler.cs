namespace MinimalApiCqrs.Simple.Todos.CompleteTodo;

using System.Threading;
using System.Threading.Tasks;
using MinimalApiCqrs.Simple.Core.CQRS;
using MinimalApiCqrs.Simple.Core.Results;
using MinimalApiCqrs.Simple.Core.Results.Errors;
using MinimalApiCqrs.Simple.Todos;

internal class CompleteTodoCommandHandler : ICommandHandler<CompleteTodoCommand>
{
    private readonly ITodoRepository todoRepository;

    public CompleteTodoCommandHandler(ITodoRepository todoRepository)
    {
        this.todoRepository = todoRepository;
    }

    public async Task<Result> Handle(
        CompleteTodoCommand request,
        CancellationToken cancellationToken
    )
    {
        var todo = await this.todoRepository.GetTodoByIdAsync(request.TodoId, cancellationToken);

        if (todo is null)
        {
            return Error.NotFound();
        }

        if (todo.IsCompleted)
        {
            return BusinessError.Conflict(
                "Todo.AlreadyCompleted",
                $"The todo {todo.Id} is already completed."
            );
        }

        todo.Complete();

        await this.todoRepository.UpdateTodoAsync(todo, cancellationToken);

        return Result.Success();
    }
}
