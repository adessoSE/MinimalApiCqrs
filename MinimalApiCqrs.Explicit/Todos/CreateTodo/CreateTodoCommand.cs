namespace MinimalApiCqrs.Simple.Todos.CreateTodo;

using MinimalApiCqrs.Simple.Core.CQRS;

public record CreateTodoCommand(string Title, string Description)
    : ICommand<CreateTodoCommandResponse>;
