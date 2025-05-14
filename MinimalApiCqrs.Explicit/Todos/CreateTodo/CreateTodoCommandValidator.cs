namespace MinimalApiCqrs.Simple.Todos.CreateTodo;

using FluentValidation;

internal class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        this.RuleFor(t => t.Title).NotEmpty().MinimumLength(5).MaximumLength(20);

        this.RuleFor(t => t.Description).NotEmpty().MaximumLength(100);
    }
}
