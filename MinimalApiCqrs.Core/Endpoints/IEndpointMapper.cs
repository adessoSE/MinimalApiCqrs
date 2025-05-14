namespace MinimalApiCqrs.Core.Endpoints;

using Microsoft.AspNetCore.Routing;

public interface IEndpointMapper
{
    static abstract void MapEndpoints(IEndpointRouteBuilder app);
}
