namespace MinimalApiCqrs.Simple.Core.CQRS;

using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiCqrs.Simple.Core.CQRS.PipelineBehaviors;

public static class CQRSSetup
{
    internal static void SetupCQRS(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);

            cfg.AddOpenBehavior(typeof(ErrorPipelineBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

        services.AddValidatorsFromAssemblies(assemblies, includeInternalTypes: true);
    }
}
