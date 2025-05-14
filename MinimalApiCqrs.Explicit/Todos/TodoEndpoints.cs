namespace MinimalApiCqrs.Simple.Todos;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using MinimalApiCqrs.Core.Endpoints;
using MinimalApiCqrs.Explicit.Core;
using MinimalApiCqrs.Explicit.Core.Handler;
using MinimalApiCqrs.Simple.Todos.CompleteTodo;
using MinimalApiCqrs.Simple.Todos.CreateTodo;
using MinimalApiCqrs.Simple.Todos.DeleteTodo;
using MinimalApiCqrs.Simple.Todos.GetTodo;
using MinimalApiCqrs.Simple.Todos.GetTodos;

public class TodoEndpoints : IEndpointMapper
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/explicit/todos").WithTags("Explicit Todos");

        group
            .RegisterGetRoute<GetTodosQuery, GetTodosQueryResponse[]>("")
            .OnSuccess(SuccessHandler.Default)
            .OnException(ExceptionHandler.Default)
            .WithName("GetExplicitTodos");

        group
            .RegisterGetRoute<GetTodoQuery, GetTodoQueryResponse>("{todoId}")
            .OnSuccess(SuccessHandler.Default)
            .OnException(ExceptionHandler.Default)
            .OnException(ExceptionHandler.NotFound)
            .WithName("GetExplicitTodo");

        group
            .RegisterPostFromBodyRoute<CreateTodoCommand, CreateTodoCommandResponse>("")
            .OnSuccess(SuccessHandler.Default)
            .OnException(ExceptionHandler.Default)
            .WithName("CreateExplicitTodo");

        group
            .RegisterPostRoute<CompleteTodoCommand>("{todoId}/complete")
            .OnSuccess(SuccessHandler.Default)
            .OnException(ExceptionHandler.Default)
            .OnException(ExceptionHandler.NotFound)
            .WithName("CompleteExplicitTodo");

        group
            .RegisterDeleteRoute<DeleteTodoCommand>("{todoId}")
            .OnSuccess(SuccessHandler.Default)
            .OnException(ExceptionHandler.Default)
            .WithName("DeleteExplicitTodo");
    }
}
