namespace MinimalApiCqrs.Explicit.Core.Mediator;

using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiCqrs.Explicit.Core.Mediator.PipelineBehaviors;

public static class CQRSSetup
{
    internal static void SetupCQRS(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);

            cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

        services.AddValidatorsFromAssemblies(assemblies, includeInternalTypes: true);
    }
}
