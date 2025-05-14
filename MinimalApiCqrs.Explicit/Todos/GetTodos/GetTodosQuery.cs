namespace MinimalApiCqrs.Simple.Todos.GetTodos;

using MinimalApiCqrs.Simple.Core.CQRS;

public sealed record GetTodosQuery : IQuery<GetTodosQueryResponse[]>;
