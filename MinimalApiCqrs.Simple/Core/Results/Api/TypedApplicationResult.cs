namespace MinimalApiCqrs.Simple.Core.Results.Api;

using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using MinimalApiCqrs.Simple.Core.Results.Errors;

public class TypedApplicationResult<TSuccessResult>
    : IResult,
        INestedHttpResult,
        IEndpointMetadataProvider
    where TSuccessResult : IResult
{
    public IResult Result { get; }

    static void IEndpointMetadataProvider.PopulateMetadata(
        MethodInfo method,
        EndpointBuilder builder
    )
    {
        Populate<
            Results<
                TSuccessResult,
                NotFound,
                ForbidHttpResult,
                ProblemHttpResult,
                ValidationProblem
            >
        >(method, builder);
    }

    private static void Populate<TStaticPopulate>(MethodInfo method, EndpointBuilder builder)
        where TStaticPopulate : IEndpointMetadataProvider
    {
        TStaticPopulate.PopulateMetadata(method, builder);
    }

    internal TypedApplicationResult(TSuccessResult? successResult, Error? error)
    {
        Result = successResult ?? CreateErrorActionResult(error!);
    }

    Task IResult.ExecuteAsync(HttpContext httpContext)
    {
        return Result.ExecuteAsync(httpContext);
    }

    private static IResult CreateErrorActionResult(Error error)
    {
        return error switch
        {
            NotFoundError or { Code: Error.Codes.NotFound } => TypedResults.NotFound(error.Message),
            { Code: Error.Codes.Forbidden } => TypedResults.Forbid(),
            TaskCanceledError canceledError => TypedResults.Json(
                canceledError.Message,
                statusCode: StatusCodes.Status408RequestTimeout
            ),
            ValidationError validationError => CreateValidationErrorResult(validationError),
            BusinessError businessError => CreateBusinessErrorResult(businessError),
            _ => CreateGenericError(error),
        };
    }

    private static ValidationProblem CreateValidationErrorResult(ValidationError validationError)
    {
        var errors = validationError.ValidationResults.ToDictionary(
            e => e.Source.Substring(0, 1).ToLower() + e.Source.Substring(1),
            e => e.ErrorMessages.ToArray()
        );

        return TypedResults.ValidationProblem(
            errors,
            detail: validationError.Message,
            extensions: new Dictionary<string, object?>()
            {
                ["errorType"] = "validationError",
                ["errorCode"] = validationError.Code,
            }
        );
    }

    private static ProblemHttpResult CreateGenericError(Error error)
    {
        return TypedResults.Problem(
            detail: error.Message,
            statusCode: StatusCodes.Status500InternalServerError,
            extensions: new Dictionary<string, object?>()
            {
                ["errorCode"] = error.Code,
                ["traceId"] = error.TraceId,
                ["occurredAt"] = error.OccurredAt,
            }
        );
    }

    private static ProblemHttpResult CreateBusinessErrorResult(BusinessError businessError)
    {
        var statusCode = businessError.ErrorType switch
        {
            BusinessErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest,
        };

        var problemExtensions = new Dictionary<string, object?>()
        {
            ["solution"] = businessError.Solution,
            ["errorType"] = "businessError",
            ["errorCode"] = businessError.Code,
            ["traceId"] = businessError.TraceId,
            ["occurredAt"] = businessError.OccurredAt,
        };

        return TypedResults.Problem(
            detail: businessError.Message,
            statusCode: statusCode,
            extensions: problemExtensions
        );
    }
}
