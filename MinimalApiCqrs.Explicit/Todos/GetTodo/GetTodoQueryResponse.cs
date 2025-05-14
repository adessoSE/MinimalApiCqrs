namespace MinimalApiCqrs.Simple.Todos.GetTodo;

using System;
using MinimalApiCqrs.Simple.Todos;

public sealed class GetTodoQueryResponse
{
    public Guid Id { get; }
    public string Title { get; }
    public string Description { get; }
    public bool IsCompleted { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }

    public GetTodoQueryResponse(Todo todo)
    {
        this.Id = todo.Id;
        this.Title = todo.Title;
        this.Description = todo.Description;
        this.IsCompleted = todo.IsCompleted;
        this.CreatedAt = todo.CreatedAt;
        this.UpdatedAt = todo.UpdatedAt;
    }
}
