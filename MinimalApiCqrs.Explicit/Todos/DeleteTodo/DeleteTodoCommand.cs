namespace MinimalApiCqrs.Simple.Todos.DeleteTodo;

using System;
using MinimalApiCqrs.Simple.Core.CQRS;

public sealed record DeleteTodoCommand(Guid TodoId) : ICommand;
