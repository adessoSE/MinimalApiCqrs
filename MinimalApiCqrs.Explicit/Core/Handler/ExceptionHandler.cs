using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MinimalApiCqrs.Explicit.Core.Context;
using MinimalApiCqrs.Explicit.Core.Exceptions;

namespace MinimalApiCqrs.Explicit.Core.Handler;

public delegate TResult ExceptionHandlerDelegate<TRequest, TException, out TResult>(
    ExceptionContext<TRequest, TException> context
)
    where TException : Exception
    where TResult : IResult;

public delegate IResult ExceptionHandlerDelegate<TRequest, TException>(
    ExceptionContext<TRequest, TException> context
)
    where TException : Exception;

public static class ExceptionHandler
{
    public static Results<
        ValidationProblem,
        Conflict<ProblemDetails>,
        ProblemHttpResult
    > Default<TRequest>(ExceptionContext<TRequest, Exception> context)
    {
        if (context.Exception is ValidationException validationException)
        {
            return TypedResults.ValidationProblem(
                validationException.ValidationErrors.ToDictionary(e => e.Property, v => v.Errors),
                title: validationException.Title,
                detail: validationException.Solution,
                extensions: GetBusinessExceptionExtendedDetails(validationException)
            );
        }

        if (context.Exception is BusinessException businessException)
        {
            context.Logger.LogInformation(businessException, businessException.Message);

            var problemResult = TypedResults.Problem(
                title: businessException.Title,
                detail: businessException.Solution,
                statusCode: StatusCodes.Status409Conflict,
                extensions: GetBusinessExceptionExtendedDetails(businessException)
            );

            return TypedResults.Conflict(problemResult.ProblemDetails);
        }

        if (context.Exception is Exception applicationException)
        {
            context.Logger.LogError(applicationException, "Exception occured");

            return TypedResults.Problem(
                title: "Unerwarteter Fehler",
                detail: $"Entschuldigung, das hätte nicht passieren dürfen. Bitte kontaktieren Sie Ihren Ansprechpartner und nennen Sie folgende ID: {context.TraceIdentifier}",
                extensions: new Dictionary<string, object?>()
                {
                    ["traceId"] = context.TraceIdentifier,
                }
            );
        }

        context.Logger.LogCritical(context.Exception, "Unexpected error occured");

        return TypedResults.Problem(
            title: "Unbekannter Fehler",
            detail: "Es ist ein unbekannter Fehler aufgetreten. Bitte versuchen Sie es später erneut.",
            extensions: new Dictionary<string, object?>() { ["traceId"] = context.TraceIdentifier }
        );
    }

    public static NotFound<ProblemDetails> NotFound<TRequest>(
        ExceptionContext<TRequest, NotFoundException> context
    )
    {
        var details = new ProblemDetails()
        {
            Title = context.Exception.Message,
            Status = StatusCodes.Status404NotFound,
        };

        return TypedResults.NotFound(details);
    }

    private static Dictionary<string, object?> GetBusinessExceptionExtendedDetails(
        BusinessException businessException
    )
    {
        var errorType = businessException switch
        {
            ValidationException => "validation",
            BusinessException => "business",
            _ => "unknown",
        };

        return new Dictionary<string, object?>()
        {
            ["severity"] = businessException.Severity.ToString(),
            ["errorType"] = errorType,
        };
    }
}
