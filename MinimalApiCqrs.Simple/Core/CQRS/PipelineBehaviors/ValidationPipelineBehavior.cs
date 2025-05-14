namespace MinimalApiCqrs.Simple.Core.CQRS.PipelineBehaviors;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MinimalApiCqrs.Simple.Core.Results;
using MinimalApiCqrs.Simple.Core.Results.Errors;

internal class ValidationPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var errors = _validators
            .Select(v => v.Validate(request))
            .SelectMany(r => r.Errors)
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .Select(g => new ValidationError.ValidationResult(
                g.Key,
                g.Distinct().ToImmutableArray()
            ))
            .ToImmutableArray();

        if (errors.Length > 0)
        {
            var validationError = new ValidationError(errors);

            return Result.Fail<TResponse>(validationError);
        }

        return await next(cancellationToken);
    }
}
