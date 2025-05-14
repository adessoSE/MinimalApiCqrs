# Introduction

This solution showcases how to build efficient Minimal APIs in .NET 8 by combining the CQRS pattern with custom Mediator extensions. The focus is on two distinct styles of endpoint registration:

- **Explicit Approach**: Uses explicit route definitions with exception-based handling hooks for fine-grained control.  
- **Simple Approach**: Employs a result-based pattern to implicitly manage errors and reduce boilerplate.

By comparing these methods side by side, you’ll understand the trade-offs in readability, error handling, and extensibility, while still generating the same OpenAPI definitions for seamless client generation.

---

## Solution Structure

This .NET 8 solution contains three projects:

- **MinimalApiCqrs.Core**  
  Contains the sample endpoint registration.

- **MinimalApiCqrs.Explicit**  
  Demonstrates the explicit approach for Minimal APIs. Exceptions are thrown from handlers and caught on the route definitions.

- **MinimalApiCqrs.Simple**  
  Shows a simpler, more implicit style using a Result pattern with error classes instead of throwing.

Each project follows this layout:

```
/<ProjectName>
  /Core      ← "wiring" of app (startup-like)
  /Todos     ← sample CRUD endpoints for a "Todo" domain
```

---

## Endpoint Mapping

### Explicit Approach

In the **Explicit** project, each endpoint definition wires up success and exception handlers by name. Exceptions thrown in handlers propagate to the route, where `.OnException()` catches them:

```csharp
public class TodoEndpoints : IEndpointMapper
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/explicit/todos")
            .WithTags("Explicit Todos");

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
```

### Simple Approach

In the **Simple** project, you register CQRS routes that return a `Result<T>` or `Result` object. Flow errors are wrapped in `Result` and handled automatically:

```csharp
public class TodoEndpoints : IEndpointMapper
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/simple/todos")
            .WithTags("Simple Todos");

        group.MapGetCQRS<GetTodosQuery, GetTodosQueryResponse[]>("");
        group.MapGetCQRS<GetTodoQuery, GetTodoQueryResponse>("{todoId}");
        group.MapPostCQRSFromBody<CreateTodoCommand, CreateTodoCommandResponse>("");
        group.MapPostCQRS<CompleteTodoCommand>("{todoId}/complete");
        group.MapDeleteCQRS<DeleteTodoCommand>("{todoId}");
    }
}
```

---

## Differences, Advantages & Drawbacks

| Aspect               | Explicit Approach                                       | Simple Approach                                            |
|----------------------|---------------------------------------------------------|------------------------------------------------------------|
| **Error Handling**   | Throws exceptions; caught per-route via `.OnException`  | Uses `Result` pattern; no exceptions for control flow      |
| **Readability**      | Verbose pipeline—clear success/exception hooks          | Concise route mapping; less boilerplate                    |
| **Consistency**      | Fine-grained control over specific exception mappings   | Uniform handling of all errors through `Result` extensions |
| **Performance**      | Minimal overhead, but exception creation is costlier    | Slightly faster in happy path (no exception allocation)    |
| **Learning Curve**   | Familiar for those used to try/catch                    | Requires understanding of `Result` and its extensions      |

---

## OpenAPI Integration

Both approaches produce nearly identical OpenAPI definitions, providing the same endpoints, request/response models, and tags—making client generation (e.g., via NSwag or AutoRest) straightforward.
