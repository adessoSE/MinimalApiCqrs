namespace MinimalApiCqrs.Core.Endpoints;

using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

public static class WebApplicationExtensions
{
    private static readonly MethodInfo MapMethod = typeof(WebApplicationExtensions).GetMethod(
        nameof(Map),
        BindingFlags.NonPublic | BindingFlags.Static
    )!;

    public static void MapEndpoints(this WebApplication app, Assembly[] assemblies)
    {
        var mapperTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IEndpointMapper).IsAssignableFrom(t));

        foreach (var mapperType in mapperTypes)
        {
            // Execute the Map<TMapper> Method with the current mapper type
            var genericMethod = MapMethod.MakeGenericMethod(mapperType);
            genericMethod.Invoke(null, new object[] { app });
        }
    }

    private static void Map<TMapper>(IEndpointRouteBuilder builder)
        where TMapper : IEndpointMapper
    {
        TMapper.MapEndpoints(builder);
    }
}
