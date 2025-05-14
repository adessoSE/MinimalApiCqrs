namespace MinimalApiCqrs.Simple.Core.Results;

using System;
using MinimalApiCqrs.Simple.Core.Results.Errors;

internal static class ErrorExceptionMapper
{
    public static Exception ToException(Error error) =>
        error switch
        {
            BusinessError businessError => new BusinessException(businessError),
            ValidationError validationError => new ValidationErrorException(validationError),
            UnexpectedError unexpectedError => new UnexpectedErrorException(
                unexpectedError.Exception
            ),
            TaskCanceledError taskCanceledError => new TaskCanceledErrorException(
                taskCanceledError.Exception
            ),
            NotFoundError notFoundError => new NotFoundException(notFoundError),
            _ when error.Code == Error.Codes.Forbidden => new ForbiddenException(),
            _ => new ErrorException(error),
        };

    public static Error ToError(Exception exception)
    {
        return exception switch
        {
            ErrorException errorException => errorException.Error,
            TaskCanceledException taskCanceledException => new TaskCanceledError(
                taskCanceledException
            ),
            OperationCanceledException operationCanceledException => new TaskCanceledError(
                operationCanceledException
            ),
            _ => new UnexpectedError(exception),
        };
    }
}
