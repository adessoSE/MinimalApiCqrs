using MediatR;
using Microsoft.AspNetCore.Http.Metadata;
using MinimalApiCqrs.Explicit.Core.Exceptions;
using MinimalApiCqrs.Explicit.Core.Handler;
using MinimalApiCqrs.Explicit.Core.Metadata;

namespace MinimalApiCqrs.Explicit.Core;

public abstract class ExplicitRouteHandlerBuilderBase<TRequest, TSubBuilder>
    where TSubBuilder : ExplicitRouteHandlerBuilderBase<TRequest, TSubBuilder>
{
    protected RouteHandlerBuilder NestedBuilder { get; }

    protected ExplicitRouteHandlerBuilderBase(RouteHandlerBuilder nestedBuilder)
    {
        NestedBuilder = nestedBuilder;

        NestedBuilder.Finally(builder =>
        {
            var successHandlerCount = builder.Metadata.Count(m => m is IOnSuccessMetadata);

            if (successHandlerCount == 0)
            {
                throw new NoSuccessHandlerException(builder.DisplayName!);
            }

            if (successHandlerCount > 1)
            {
                throw new TooManySuccessHandlerException(builder.DisplayName!);
            }

            if (!builder.Metadata.Any(m => m is IOnExceptionMetadata))
            {
                throw new NoExceptionHandlerException(builder.DisplayName!);
            }
        });
    }

    public TSubBuilder OnException<TResult>(
        ExceptionHandlerDelegate<TRequest, Exception, TResult> action
    )
        where TResult : IResult, IEndpointMetadataProvider
    {
        return OnException<Exception, TResult>(action);
    }

    public TSubBuilder OnException<TResult>(
        ExceptionHandlerDelegate<TRequest, NotFoundException, TResult> action
    )
        where TResult : IResult, IEndpointMetadataProvider
    {
        return OnException<NotFoundException, TResult>(action);
    }

    public TSubBuilder OnException<TException, TResult>(
        ExceptionHandlerDelegate<TRequest, TException, TResult> action
    )
        where TException : Exception
        where TResult : IResult, IEndpointMetadataProvider
    {
        NestedBuilder.WithMetadata(
            new OnExceptionMetadata<TRequest, TException>(context => action(context))
        );

        AddResultTypeMetadata<TResult>();

        return (TSubBuilder)this;
    }

    public TSubBuilder OnException<TException, TResult>(
        ExceptionHandlerDelegate<TRequest, TException, TResult> action,
        int statusCode,
        Type? responseType = null,
        string? contentType = null,
        params string[] additionalContentTypes
    )
        where TResult : IResult
        where TException : Exception
    {
        NestedBuilder.WithMetadata(
            new OnExceptionMetadata<TRequest, TException>(context => action(context))
        );

        NestedBuilder.Produces(statusCode, responseType, contentType, additionalContentTypes);

        return (TSubBuilder)this;
    }

    public RouteHandlerBuilder WithName(string name)
    {
        return NestedBuilder
            .WithName(name)
            .WithOpenApi(configure =>
            {
                configure.OperationId = $"{name}Async";
                return configure;
            });
    }

    protected void AddResultTypeMetadata<TResult>(EndpointBuilder? endpointBuilder = null)
        where TResult : IResult, IEndpointMetadataProvider
    {
        if (endpointBuilder is not null)
        {
            TResult.PopulateMetadata(endpointBuilder.RequestDelegate!.Method, endpointBuilder);
        }
        else
        {
            NestedBuilder.Add(eb =>
            {
                TResult.PopulateMetadata(eb.RequestDelegate!.Method, eb);
            });
        }
    }
}

public class ExplicitRouteHandlerBuilder<TRequest>
    : ExplicitRouteHandlerBuilderBase<TRequest, ExplicitRouteHandlerBuilder<TRequest>>
    where TRequest : IRequest
{
    public ExplicitRouteHandlerBuilder(RouteHandlerBuilder nestedBuilder)
        : base(nestedBuilder) { }

    public ExplicitRouteHandlerBuilder<TRequest> OnSuccess<TResult>(
        SuccessHandlerDelegate<TRequest, TResult> action
    )
        where TResult : IResult, IEndpointMetadataProvider
    {
        NestedBuilder.WithMetadata(new OnSuccessMetadata<TRequest>(context => action(context)));

        AddResultTypeMetadata<TResult>();

        return this;
    }

    public ExplicitRouteHandlerBuilder<TRequest> OnSuccess<TResult>(
        SuccessHandlerDelegate<TRequest, TResult> action,
        int statusCode,
        Type? responseType = null,
        string? contentType = null,
        params string[] additionalContentTypes
    )
        where TResult : IResult
    {
        NestedBuilder.WithMetadata(new OnSuccessMetadata<TRequest>(context => action(context)));

        NestedBuilder.Produces(statusCode, responseType, contentType, additionalContentTypes);

        return this;
    }
}

public class ExplicitRouteHandlerBuilder<TRequest, TResponse>
    : ExplicitRouteHandlerBuilderBase<TRequest, ExplicitRouteHandlerBuilder<TRequest, TResponse>>
    where TRequest : IRequest<TResponse>
{
    public ExplicitRouteHandlerBuilder(RouteHandlerBuilder nestedBuilder)
        : base(nestedBuilder) { }

    public ExplicitRouteHandlerBuilder<TRequest, TResponse> OnSuccess<TResult>(
        SuccessHandlerWithResponseDelegate<TRequest, TResponse, TResult> action
    )
        where TResult : IResult, IEndpointMetadataProvider
    {
        NestedBuilder.WithMetadata(
            new OnSuccessMetadata<TRequest, TResponse>(context => action(context))
        );

        AddResultTypeMetadata<TResult>();

        return this;
    }

    public ExplicitRouteHandlerBuilder<TRequest, TResponse> OnSuccess<TResult>(
        SuccessHandlerWithResponseDelegate<TRequest, TResponse, TResult> action,
        int statusCode,
        Type? responseType = null,
        string? contentType = null,
        params string[] additionalContentTypes
    )
        where TResult : IResult
    {
        NestedBuilder.WithMetadata(
            new OnSuccessMetadata<TRequest, TResponse>(context => action(context))
        );

        NestedBuilder.Produces(statusCode, responseType, contentType, additionalContentTypes);

        return this;
    }
}
