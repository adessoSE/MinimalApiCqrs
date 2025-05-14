namespace MinimalApiCqrs.Explicit.Core.Mediator.PipelineBehaviors;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MinimalApiCqrs.Explicit.Core.Exceptions;
using ValidationException = Exceptions.ValidationException;

internal class ValidationPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        this._validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (!this._validators.Any())
        {
            return await next();
        }

        var validationTasks = this._validators.Select(v => v.ValidateAsync(request)).ToArray();

        var validationResults = await Task.WhenAll(validationTasks);

        var errors = validationResults
            .SelectMany(r => r.Errors)
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .Select(g => new ValidationExceptionError(g.Key, g.Distinct().ToArray()))
            .ToArray();

        if (errors.Length > 0)
        {
            throw new ValidationException(errors);
        }

        return await next();
    }
}
