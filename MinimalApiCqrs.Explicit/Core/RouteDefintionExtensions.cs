using System.Diagnostics.CodeAnalysis;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MinimalApiCqrs.Explicit.Core.Context;
using MinimalApiCqrs.Explicit.Core.Metadata;

namespace MinimalApiCqrs.Explicit.Core;

public static class RouteDefintionExtensions
{
    public static ExplicitRouteHandlerBuilder<TRequest, TResponse> RegisterGetRoute<
        TRequest,
        TResponse
    >(this IEndpointRouteBuilder app, [StringSyntax("Route")] string routeTemplate)
        where TRequest : IRequest<TResponse>
    {
        return new(app.MapGet(routeTemplate, RouteHandlerAsParametersAsync<TRequest, TResponse>));
    }

    public static ExplicitRouteHandlerBuilder<TRequest> RegisterPostRoute<TRequest>(
        this IEndpointRouteBuilder app,
        [StringSyntax("Route")] string routeTemplate
    )
        where TRequest : IRequest
    {
        return new(app.MapPost(routeTemplate, RouteHandlerAsParametersAsync<TRequest>));
    }

    public static ExplicitRouteHandlerBuilder<TRequest> RegisterPostFromBodyRoute<TRequest>(
        this IEndpointRouteBuilder app,
        [StringSyntax("Route")] string routeTemplate
    )
        where TRequest : IRequest
    {
        return new(app.MapPost(routeTemplate, RouteHandlerFromBodyAsync<TRequest>));
    }

    public static ExplicitRouteHandlerBuilder<TRequest, TResponse> RegisterPostRoute<
        TRequest,
        TResponse
    >(this IEndpointRouteBuilder app, [StringSyntax("Route")] string routeTemplate)
        where TRequest : IRequest<TResponse>
    {
        return new(app.MapPost(routeTemplate, RouteHandlerAsParametersAsync<TRequest, TResponse>));
    }

    public static ExplicitRouteHandlerBuilder<TRequest, TResponse> RegisterPostFromBodyRoute<
        TRequest,
        TResponse
    >(this IEndpointRouteBuilder app, [StringSyntax("Route")] string routeTemplate)
        where TRequest : IRequest<TResponse>
    {
        return new(app.MapPost(routeTemplate, RouteHandlerFromBodyAsync<TRequest, TResponse>));
    }

    public static ExplicitRouteHandlerBuilder<TRequest> RegisterPutRoute<TRequest>(
        this IEndpointRouteBuilder app,
        [StringSyntax("Route")] string routeTemplate
    )
        where TRequest : IRequest
    {
        return new(app.MapPut(routeTemplate, RouteHandlerAsParametersAsync<TRequest>));
    }

    public static ExplicitRouteHandlerBuilder<TRequest> RegisterPutFromBodyRoute<TRequest>(
        this IEndpointRouteBuilder app,
        [StringSyntax("Route")] string routeTemplate
    )
        where TRequest : IRequest
    {
        return new(app.MapPut(routeTemplate, RouteHandlerFromBodyAsync<TRequest>));
    }

    public static ExplicitRouteHandlerBuilder<TRequest, TResponse> RegisterPutRoute<
        TRequest,
        TResponse
    >(this IEndpointRouteBuilder app, [StringSyntax("Route")] string routeTemplate)
        where TRequest : IRequest<TResponse>
    {
        return new(app.MapPut(routeTemplate, RouteHandlerAsParametersAsync<TRequest, TResponse>));
    }

    public static ExplicitRouteHandlerBuilder<TRequest, TResponse> RegisterPutFromBodyRoute<
        TRequest,
        TResponse
    >(this IEndpointRouteBuilder app, [StringSyntax("Route")] string routeTemplate)
        where TRequest : IRequest<TResponse>
    {
        return new(app.MapPut(routeTemplate, RouteHandlerFromBodyAsync<TRequest, TResponse>));
    }

    public static ExplicitRouteHandlerBuilder<TRequest> RegisterDeleteRoute<TRequest>(
        this IEndpointRouteBuilder app,
        [StringSyntax("Route")] string routeTemplate
    )
        where TRequest : IRequest
    {
        return new(app.MapDelete(routeTemplate, RouteHandlerAsParametersAsync<TRequest>));
    }

    public static ExplicitRouteHandlerBuilder<TRequest, TResponse> RegisterDeleteRoute<
        TRequest,
        TResponse
    >(this IEndpointRouteBuilder app, [StringSyntax("Route")] string routeTemplate)
        where TRequest : IRequest<TResponse>
    {
        return new(
            app.MapDelete(routeTemplate, RouteHandlerAsParametersAsync<TRequest, TResponse>)
        );
    }

    private static Task<IResult> RouteHandlerAsParametersAsync<TRequest, TResponse>(
        [FromServices] IMediator mediator,
        [AsParameters] TRequest request,
        HttpContext context,
        CancellationToken cancellationToken
    )
        where TRequest : IRequest<TResponse>
    {
        return RouteHandlerAsync<TRequest, TResponse>(
            mediator,
            request,
            context,
            cancellationToken
        );
    }

    private static Task<IResult> RouteHandlerFromBodyAsync<TRequest, TResponse>(
        [FromServices] IMediator mediator,
        [FromBody] TRequest request,
        HttpContext context,
        CancellationToken cancellationToken
    )
        where TRequest : IRequest<TResponse>
    {
        return RouteHandlerAsync<TRequest, TResponse>(
            mediator,
            request,
            context,
            cancellationToken
        );
    }

    private static async Task<IResult> RouteHandlerAsync<TRequest, TResponse>(
        IMediator mediator,
        TRequest request,
        HttpContext context,
        CancellationToken cancellationToken
    )
        where TRequest : IRequest<TResponse>
    {
        var endpoint = context.GetEndpoint();

        if (endpoint is null)
        {
            return Results.NotFound();
        }

        try
        {
            var response = await mediator.Send(request, cancellationToken);

            var actionMetadata = endpoint.Metadata.GetMetadata<
                OnSuccessMetadata<TRequest, TResponse>
            >()!;

            var logger = context.RequestServices.GetRequiredService<
                ILogger<SuccessContext<TRequest, TResponse>>
            >();
            var successContext = new SuccessContext<TRequest, TResponse>(
                request,
                response,
                logger,
                context
            );

            var httpResult = actionMetadata.Action.Invoke(successContext);

            return httpResult;
        }
        catch (TaskCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return HandleException(context, endpoint, request, ex);
        }
    }

    private static Task<IResult> RouteHandlerAsParametersAsync<TRequest>(
        [FromServices] IMediator mediator,
        [AsParameters] TRequest request,
        HttpContext context,
        CancellationToken cancellationToken
    )
        where TRequest : IRequest
    {
        return RouteHandlerAsync(mediator, request, context, cancellationToken);
    }

    private static Task<IResult> RouteHandlerFromBodyAsync<TRequest>(
        [FromServices] IMediator mediator,
        [FromBody] TRequest request,
        HttpContext context,
        CancellationToken cancellationToken
    )
        where TRequest : IRequest
    {
        return RouteHandlerAsync(mediator, request, context, cancellationToken);
    }

    private static async Task<IResult> RouteHandlerAsync<TRequest>(
        IMediator mediator,
        TRequest request,
        HttpContext context,
        CancellationToken cancellationToken
    )
        where TRequest : IRequest
    {
        var endpoint = context.GetEndpoint();

        if (endpoint is null)
        {
            return Results.NotFound();
        }

        try
        {
            await mediator.Send(request, cancellationToken);

            var actionMetadata = endpoint.Metadata.GetMetadata<OnSuccessMetadata<TRequest>>()!;

            var logger = context.RequestServices.GetRequiredService<
                ILogger<SuccessContext<TRequest>>
            >();
            var successContext = new SuccessContext<TRequest>(request, logger, context);

            var httpResult = actionMetadata.Action.Invoke(successContext);

            return httpResult;
        }
        catch (TaskCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return HandleException(context, endpoint, request, ex);
        }
    }

    private static IResult HandleException<TRequest>(
        HttpContext context,
        Endpoint endpoint,
        TRequest request,
        Exception exception
    )
    {
        var exceptionHandler = endpoint
            .Metadata.GetOrderedMetadata<IOnExceptionMetadata>()
            .OrderByDescending(m => GetInheritanceDepth(m.ExceptionType))
            .FirstOrDefault(metadata => metadata.ExceptionType.IsInstanceOfType(exception));

        var result = exceptionHandler?.MapExceptionToResult(request!, exception, context);

        if (result is not null)
        {
            return result;
        }

        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }

    private static int GetInheritanceDepth(Type? type)
    {
        var depth = 0;
        var objectType = typeof(object);

        while (type is not null && type != objectType)
        {
            depth++;
            type = type.BaseType;
        }

        return depth;
    }
}
