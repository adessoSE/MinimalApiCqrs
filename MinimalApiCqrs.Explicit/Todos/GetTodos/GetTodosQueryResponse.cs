namespace MinimalApiCqrs.Simple.Todos.GetTodos;

using System;

public sealed record GetTodosQueryResponse(
    Guid Id,
    string Title,
    string Description,
    bool IsCompleted,
    DateTime UpdatedAt
);
