namespace MinimalApiCqrs.Simple.Todos.CompleteTodo;

using System;
using MinimalApiCqrs.Simple.Core.CQRS;

public sealed record CompleteTodoCommand(Guid TodoId) : ICommand;
