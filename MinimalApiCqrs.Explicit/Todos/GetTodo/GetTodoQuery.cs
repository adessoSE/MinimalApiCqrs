namespace MinimalApiCqrs.Simple.Todos.GetTodo;

using System;
using MinimalApiCqrs.Simple.Core.CQRS;

public sealed record GetTodoQuery(Guid TodoId) : IQuery<GetTodoQueryResponse>;
