namespace MinimalApiCqrs.Simple.Todos.GetTodo;

using System.Threading;
using System.Threading.Tasks;
using MinimalApiCqrs.Simple.Core.CQRS;
using MinimalApiCqrs.Simple.Core.Results;
using MinimalApiCqrs.Simple.Core.Results.Errors;
using MinimalApiCqrs.Simple.Todos;

internal class GetTodoQueryHandler : IQueryHandler<GetTodoQuery, GetTodoQueryResponse>
{
    private readonly ITodoRepository todoRepository;

    public GetTodoQueryHandler(ITodoRepository todoRepository)
    {
        this.todoRepository = todoRepository;
    }

    public async Task<Result<GetTodoQueryResponse>> Handle(
        GetTodoQuery request,
        CancellationToken cancellationToken
    )
    {
        var todo = await this.todoRepository.GetTodoByIdAsync(request.TodoId, cancellationToken);

        if (todo is null)
        {
            return Error.NotFound();
        }

        return new GetTodoQueryResponse(todo);
    }
}
