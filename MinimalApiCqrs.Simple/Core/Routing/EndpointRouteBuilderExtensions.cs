namespace MinimalApiCqrs.Simple.Core.Routing;

using System.Diagnostics.CodeAnalysis;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MinimalApiCqrs.Simple.Core.CQRS;
using MinimalApiCqrs.Simple.Core.Results;
using MinimalApiCqrs.Simple.Core.Results.Api;

public static class EndpointRouteBuilderExtensions
{
    private static RouteHandlerBuilder ConfigureEndpointGlobal<T>(
        this RouteHandlerBuilder endpoints
    )
    {
        var name = typeof(T)
            .Name.Replace("Query", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("Command", string.Empty, StringComparison.OrdinalIgnoreCase);

        return endpoints.WithName(name);
    }

    public static RouteHandlerBuilder MapGetCQRS<TQuery, TResult>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern,
        Func<TQuery, TResult, IResult>? resultTransform = null
    )
        where TQuery : IQuery<TResult>
        where TResult : notnull
    {
        return endpoints.MapQuery(pattern, useBody: false, resultTransform);
    }

    public static RouteHandlerBuilder MapGetWithBodyCQRS<TQuery, TResult>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern,
        Func<TQuery, TResult, IResult>? resultTransform = null
    )
        where TQuery : IQuery<TResult>
        where TResult : notnull
    {
        return endpoints.MapQuery(pattern, useBody: true, resultTransform);
    }

    public static RouteHandlerBuilder MapPostCQRS<TCommand>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern
    )
        where TCommand : ICommand
    {
        return endpoints.MapCommand<TCommand>(pattern, useBody: false, HttpMethods.Post);
    }

    public static RouteHandlerBuilder MapPostCQRSFromBody<TCommand>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern
    )
        where TCommand : ICommand
    {
        return endpoints.MapCommand<TCommand>(pattern, useBody: true, HttpMethods.Post);
    }

    public static RouteHandlerBuilder MapPostCQRS<TCommand, TResult>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern,
        Func<TCommand, TResult, IResult>? resultTransform = null
    )
        where TCommand : ICommand<TResult>
        where TResult : notnull
    {
        return endpoints.MapCommand(pattern, useBody: false, HttpMethods.Post, resultTransform);
    }

    public static RouteHandlerBuilder MapPostCQRSFromBody<TCommand, TResult>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern,
        Func<TCommand, TResult, IResult>? resultTransform = null
    )
        where TCommand : ICommand<TResult>
        where TResult : notnull
    {
        return endpoints.MapCommand(pattern, useBody: true, HttpMethods.Post, resultTransform);
    }

    public static RouteHandlerBuilder MapPutCQRS<TCommand>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern
    )
        where TCommand : ICommand
    {
        return endpoints.MapCommand<TCommand>(pattern, useBody: false, HttpMethods.Put);
    }

    public static RouteHandlerBuilder MapPutCQRSFromBody<TCommand>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern
    )
        where TCommand : ICommand
    {
        return endpoints.MapCommand<TCommand>(pattern, useBody: true, HttpMethods.Put);
    }

    public static RouteHandlerBuilder MapPutCQRS<TCommand, TResult>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern,
        Func<TCommand, TResult, IResult>? resultTransform = null
    )
        where TCommand : ICommand<TResult>
        where TResult : notnull
    {
        return endpoints.MapCommand(pattern, useBody: false, HttpMethods.Put, resultTransform);
    }

    public static RouteHandlerBuilder MapPutCQRSFromBody<TCommand, TResult>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern,
        Func<TCommand, TResult, IResult>? resultTransform = null
    )
        where TCommand : ICommand<TResult>
        where TResult : notnull
    {
        return endpoints.MapCommand(pattern, useBody: true, HttpMethods.Put, resultTransform);
    }

    public static RouteHandlerBuilder MapPatchCQRS<TCommand>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern
    )
        where TCommand : ICommand
    {
        return endpoints.MapCommand<TCommand>(pattern, useBody: false, HttpMethods.Patch);
    }

    public static RouteHandlerBuilder MapPatchCQRSFromBody<TCommand>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern
    )
        where TCommand : ICommand
    {
        return endpoints.MapCommand<TCommand>(pattern, useBody: true, HttpMethods.Patch);
    }

    public static RouteHandlerBuilder MapPatchCQRS<TCommand, TResult>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern,
        Func<TCommand, TResult, IResult>? resultTransform = null
    )
        where TCommand : ICommand<TResult>
        where TResult : notnull
    {
        return endpoints.MapCommand(pattern, useBody: false, HttpMethods.Patch, resultTransform);
    }

    public static RouteHandlerBuilder MapPatchCQRSFromBody<TCommand, TResult>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern,
        Func<TCommand, TResult, IResult>? resultTransform = null
    )
        where TCommand : ICommand<TResult>
        where TResult : notnull
    {
        return endpoints.MapCommand(pattern, useBody: true, HttpMethods.Patch, resultTransform);
    }

    public static RouteHandlerBuilder MapDeleteCQRS<TCommand>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern
    )
        where TCommand : ICommand
    {
        return endpoints.MapCommand<TCommand>(pattern, useBody: false, HttpMethods.Delete);
    }

    public static RouteHandlerBuilder MapDeleteCQRSFromBody<TCommand>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern
    )
        where TCommand : ICommand
    {
        return endpoints.MapCommand<TCommand>(pattern, useBody: true, HttpMethods.Delete);
    }

    public static RouteHandlerBuilder MapDeleteCQRS<TCommand, TResult>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern
    )
        where TCommand : ICommand<TResult>
        where TResult : notnull
    {
        return endpoints.MapCommand<TCommand, TResult>(
            pattern,
            useBody: false,
            HttpMethods.Delete,
            null
        );
    }

    public static RouteHandlerBuilder MapDeleteCQRSFromBody<TCommand, TResult>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern,
        Func<TCommand, TResult, IResult>? resultTransform = null
    )
        where TCommand : ICommand<TResult>
        where TResult : notnull
    {
        return endpoints.MapCommand(pattern, useBody: true, HttpMethods.Delete, resultTransform);
    }

    private static RouteHandlerBuilder MapQuery<TQuery, TResult>(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        bool useBody,
        Func<TQuery, TResult, IResult>? resultTransform
    )
        where TQuery : IQuery<TResult>
        where TResult : notnull
    {
        RouteHandlerBuilder? builder = null;

        if (useBody)
        {
            if (resultTransform is not null)
            {
                builder = endpoints.MapPost(
                    pattern,
                    async (
                        [FromBody] TQuery query,
                        [FromServices] IMediator mediator,
                        CancellationToken cancellationToken
                    ) =>
                    {
                        var result = await mediator.Send(query, cancellationToken);

                        if (result.IsFailure)
                        {
                            return result.ToTypedApiResult();
                        }

                        return resultTransform(query, result.Value);
                    }
                );
            }
            else
            {
                builder = endpoints.MapPost(
                    pattern,
                    async (
                        [FromBody] TQuery query,
                        [FromServices] IMediator mediator,
                        CancellationToken cancellationToken
                    ) =>
                    {
                        var result = await mediator.Send(query, cancellationToken);

                        return result.ToTypedApiResult();
                    }
                );
            }
        }
        else
        {
            if (resultTransform is not null)
            {
                builder = endpoints.MapGet(
                    pattern,
                    async (
                        [AsParameters] TQuery query,
                        [FromServices] IMediator mediator,
                        CancellationToken cancellationToken
                    ) =>
                    {
                        var result = await mediator.Send(query, cancellationToken);

                        if (result.IsFailure)
                        {
                            return result.ToTypedApiResult();
                        }

                        return resultTransform(query, result.Value);
                    }
                );
            }
            else
            {
                builder = endpoints.MapGet(
                    pattern,
                    async (
                        [AsParameters] TQuery query,
                        [FromServices] IMediator mediator,
                        CancellationToken cancellationToken
                    ) =>
                    {
                        var result = await mediator.Send(query, cancellationToken);

                        return result.ToTypedApiResult();
                    }
                );
            }
        }

        return builder.ConfigureEndpointGlobal<TQuery>();
    }

    private static RouteHandlerBuilder MapCommand<TCommand, TResult>(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        bool useBody,
        string httpMethod,
        Func<TCommand, TResult, IResult>? resultTransform
    )
        where TCommand : IRequest<Result<TResult>>
        where TResult : notnull
    {
        Func<string, Delegate, RouteHandlerBuilder> map = httpMethod switch
        {
            "POST" => endpoints.MapPost,
            "PUT" => endpoints.MapPut,
            "PATCH" => endpoints.MapPatch,
            "DELETE" => endpoints.MapDelete,
            _ => throw new NotSupportedException(
                $"HTTP '{httpMethod}' not supported for generic commands"
            ),
        };

        RouteHandlerBuilder? builder = null;

        if (useBody)
        {
            if (resultTransform is not null)
            {
                builder = map(
                    pattern,
                    async (
                        [FromBody] TCommand command,
                        [FromServices] IMediator mediator,
                        CancellationToken cancellationToken
                    ) =>
                    {
                        var result = await mediator.Send(command, cancellationToken);

                        if (result.IsFailure)
                        {
                            return result.ToTypedApiResult();
                        }

                        return resultTransform(command, result.Value);
                    }
                );
            }
            else
            {
                builder = map(
                    pattern,
                    async (
                        [FromBody] TCommand command,
                        IMediator mediator,
                        CancellationToken cancellationToken
                    ) =>
                    {
                        var result = await mediator.Send(command, cancellationToken);

                        return result.ToTypedApiResult();
                    }
                );
            }
        }
        else
        {
            if (resultTransform is not null)
            {
                builder = map(
                    pattern,
                    async (
                        [AsParameters] TCommand command,
                        IMediator mediator,
                        CancellationToken cancellationToken
                    ) =>
                    {
                        var result = await mediator.Send(command, cancellationToken);
                        if (result.IsFailure)
                        {
                            return result.ToTypedApiResult();
                        }

                        return resultTransform(command, result.Value);
                    }
                );
            }
            else
            {
                builder = map(
                    pattern,
                    async (
                        [AsParameters] TCommand command,
                        IMediator mediator,
                        CancellationToken cancellationToken
                    ) =>
                    {
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToTypedApiResult();
                    }
                );
            }
        }

        return builder.ConfigureEndpointGlobal<TCommand>();
    }

    private static RouteHandlerBuilder MapCommand<TCommand>(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        bool useBody,
        string httpMethod
    )
        where TCommand : IRequest<Result>
    {
        Func<string, Delegate, RouteHandlerBuilder> map = httpMethod switch
        {
            "POST" => endpoints.MapPost,
            "PUT" => endpoints.MapPut,
            "PATCH" => endpoints.MapPatch,
            "DELETE" => endpoints.MapDelete,
            _ => throw new NotSupportedException(
                $"HTTP '{httpMethod}' not supported for non-generic commands"
            ),
        };

        RouteHandlerBuilder? builder = null;

        if (useBody)
        {
            builder = map(
                pattern,
                async (
                    [FromBody] TCommand command,
                    [FromServices] IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await mediator.Send(command, cancellationToken);
                    return result.ToTypedApiResult();
                }
            );
        }
        else
        {
            builder = map(
                pattern,
                async (
                    [AsParameters] TCommand command,
                    [FromServices] IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await mediator.Send(command, cancellationToken);
                    return result.ToTypedApiResult();
                }
            );
        }

        return builder.ConfigureEndpointGlobal<TCommand>();
    }
}
