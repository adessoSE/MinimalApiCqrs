namespace MinimalApiCqrs.Simple.Core.CQRS.PipelineBehaviors;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using MinimalApiCqrs.Simple.Core.Results;
using MinimalApiCqrs.Simple.Core.Results.Errors;

internal class ErrorPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly ILogger<TRequest> _logger;

    public ErrorPipelineBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var result = await next(cancellationToken);

            if (result.IsFailure)
            {
                LogError(result.Error);
            }

            return result;
        }
        catch (Exception ex)
        {
            var error = ErrorExceptionMapper.ToError(ex);

            LogError(error);

            return Result.Fail<TResponse>(error);
        }
    }

    private void LogError(Error error)
    {
        var scopeMetadata = new Dictionary<string, object>
        {
            { "RequestType", typeof(TRequest).Name },
        };

        using (_logger.BeginScope(scopeMetadata))
        {
            error.Log(_logger);
        }
    }
}
