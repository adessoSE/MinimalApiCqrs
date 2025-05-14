namespace MinimalApiCqrs.Simple.Todos.GetTodo;

using FluentValidation;

internal class GetTodoQueryValidator : AbstractValidator<GetTodoQuery>
{
    public GetTodoQueryValidator()
    {
        this.RuleFor(t => t.TodoId).NotEmpty();
    }
}
