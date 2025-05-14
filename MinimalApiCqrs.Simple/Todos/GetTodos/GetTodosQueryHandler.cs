namespace MinimalApiCqrs.Simple.Todos.GetTodos;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MinimalApiCqrs.Simple.Core.CQRS;
using MinimalApiCqrs.Simple.Core.Results;
using MinimalApiCqrs.Simple.Todos;

internal class GetTodosQueryHandler : IQueryHandler<GetTodosQuery, GetTodosQueryResponse[]>
{
    private readonly ITodoRepository todoRepository;

    public GetTodosQueryHandler(ITodoRepository todoRepository)
    {
        this.todoRepository = todoRepository;
    }

    public async Task<Result<GetTodosQueryResponse[]>> Handle(
        GetTodosQuery request,
        CancellationToken cancellationToken
    )
    {
        var todos = await this.todoRepository.GetTodosAsync(cancellationToken);

        var result = todos
            .Select(todo => new GetTodosQueryResponse(
                todo.Id,
                todo.Title,
                todo.Description,
                todo.IsCompleted,
                todo.UpdatedAt
            ))
            .ToArray();

        return result;
    }
}
