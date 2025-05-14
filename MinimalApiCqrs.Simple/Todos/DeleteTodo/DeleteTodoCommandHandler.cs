namespace MinimalApiCqrs.Simple.Todos.DeleteTodo;

using System.Threading;
using System.Threading.Tasks;
using MinimalApiCqrs.Simple.Core.CQRS;
using MinimalApiCqrs.Simple.Core.Results;
using MinimalApiCqrs.Simple.Todos;

internal class DeleteTodoCommandHandler : ICommandHandler<DeleteTodoCommand>
{
    private readonly ITodoRepository todoRepository;

    public DeleteTodoCommandHandler(ITodoRepository todoRepository)
    {
        this.todoRepository = todoRepository;
    }

    public async Task<Result> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        await this.todoRepository.DeleteTodoAsync(request.TodoId, cancellationToken);

        return Result.Success();
    }
}
