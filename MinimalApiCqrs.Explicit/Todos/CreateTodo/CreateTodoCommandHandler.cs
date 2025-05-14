namespace MinimalApiCqrs.Simple.Todos.CreateTodo;

using System.Threading;
using System.Threading.Tasks;
using MinimalApiCqrs.Simple.Core.CQRS;
using MinimalApiCqrs.Simple.Todos;

internal class CreateTodoCommandHandler
    : ICommandHandler<CreateTodoCommand, CreateTodoCommandResponse>
{
    private readonly ITodoRepository todoRepository;

    public CreateTodoCommandHandler(ITodoRepository todoRepository)
    {
        this.todoRepository = todoRepository;
    }

    public async Task<CreateTodoCommandResponse> Handle(
        CreateTodoCommand request,
        CancellationToken cancellationToken
    )
    {
        var todo = new Todo(request.Title, request.Description);

        await this.todoRepository.AddTodoAsync(todo, cancellationToken);

        return new CreateTodoCommandResponse(todo.Id);
    }
}
