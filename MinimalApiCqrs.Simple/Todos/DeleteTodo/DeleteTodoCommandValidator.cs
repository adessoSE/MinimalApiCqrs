namespace MinimalApiCqrs.Simple.Todos.DeleteTodo;

using FluentValidation;

internal class DeleteTodoCommandValidator : AbstractValidator<DeleteTodoCommand>
{
    public DeleteTodoCommandValidator()
    {
        RuleFor(x => x.TodoId).NotEmpty();
    }
}
