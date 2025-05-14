namespace MinimalApiCqrs.Simple.Todos.CompleteTodo;

using FluentValidation;

internal class CompleteTodoCommandValidator : AbstractValidator<CompleteTodoCommand>
{
    public CompleteTodoCommandValidator()
    {
        RuleFor(x => x.TodoId).NotEmpty();
    }
}
