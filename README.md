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

## Command and Query Structure

The project follows the Command Query Responsibility Segregation (CQRS) pattern, which separates the operations that change data from the operations that read data. The CQRS pattern helps to create a more scalable, maintainable, and testable architecture.

The structure of a command or query follows the naming convention:

- `[NameOfOperation]Command.cs`
- `[NameOfOperation]CommandHandler.cs`
- `[NameOfOperation]CommandResponse.cs` (optional)
- `[NameOfOperation]CommandValidator.cs` (optional)

For queries, only the validator is optional.

### Commands

Commands represent operations that change data, such as creating, updating, or deleting entities. Each command consists of four parts:

1. **Command**: A class that encapsulates the data required for the operation. It should be a plain old C# object (POCO) with properties that match the data that needs to be sent.

2. **Command Handler**: A class that implements the `ICommandHandler<TCommand, TResult>` interface and contains the logic to perform the operation. The command handler is responsible for loading any necessary data, validating the input, and updating the data store.

3. **Command Response** (optional): A class that encapsulates the result of the operation. It should be a plain old C# object (POCO) with properties that match the data that needs to be returned.

4. **Command Validator** (optional): A class that uses Fluent Validation, a popular .NET library for building strongly-typed validation rules, to validate the input data for the command. The validator should be a separate class to keep the validation logic separate from the command logic.

### Queries

Queries represent operations that read data, such as retrieving a list of entities or a single entity. Each query consists of four parts:

1. **Query**: A class that encapsulates the data required for the operation. It should be a plain old C# object (POCO) with properties that match the data that needs to be sent.

2. **Query Handler**: A class that implements the `IQueryHandler<TQuery, TResult>` interface and contains the logic to perform the operation. The query handler is responsible for loading any necessary data, validating the input, and returning the result.

3. **Query Response**: A class that encapsulates the result of the operation. It should be a plain old C# object (POCO) with properties that match the data that needs to be returned.

4. **Query Validator** (optional): A class that uses Fluent Validation to validate the input data for the query. The validator should be a separate class to keep the validation logic separate from the query logic.

## FluentValidation

The project uses FluentValidation, which is a popular .NET library for building strongly-typed validation rules. The project includes an example of a validator for the `CreateTodoCommand` in the `CreateTodoCommandValidator.cs` file.

```csharp
internal class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
 public CreateTodoCommandValidator()
 {
  this.RuleFor(t => t.Title)
   .NotEmpty()
   .MinimumLength(5)
   .MaximumLength(20);

  this.RuleFor(t => t.Description)
   .NotEmpty()
   .MaximumLength(100);
 }
}
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
