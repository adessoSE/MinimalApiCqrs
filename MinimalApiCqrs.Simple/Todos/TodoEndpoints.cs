namespace MinimalApiCqrs.Simple.Todos;

using Microsoft.AspNetCore.Routing;
using MinimalApiCqrs.Core.Endpoints;
using MinimalApiCqrs.Simple.Core.Routing;
using MinimalApiCqrs.Simple.Todos.CompleteTodo;
using MinimalApiCqrs.Simple.Todos.CreateTodo;
using MinimalApiCqrs.Simple.Todos.DeleteTodo;
using MinimalApiCqrs.Simple.Todos.GetTodo;
using MinimalApiCqrs.Simple.Todos.GetTodos;

public class TodoEndpoints : IEndpointMapper
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/simple/todos").WithTags("Simple Todos");

        group.MapGetCQRS<GetTodosQuery, GetTodosQueryResponse[]>("");
        group.MapGetCQRS<GetTodoQuery, GetTodoQueryResponse>("{todoId}");
        group.MapPostCQRSFromBody<CreateTodoCommand, CreateTodoCommandResponse>("");
        group.MapPostCQRS<CompleteTodoCommand>("{todoId}/complete");
        group.MapDeleteCQRS<DeleteTodoCommand>("{todoId}");
    }
}
