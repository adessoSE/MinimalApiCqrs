namespace MinimalApiCqrs.Simple.Core.Routing;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

public static class EndpointRouteBuilderFileExtensions
{
    public static TBuilder WithFileUpload<TBuilder>(
        this TBuilder builder,
        long maxFileUploadLimitInBytes
    )
        where TBuilder : IEndpointConventionBuilder
    {
        return builder
            .DisableAntiforgery()
            .WithMetadata(
                new DisableRequestSizeLimitAttribute() // Disable Kestrel request limit
            )
            .WithMetadata(
                new RequestFormLimitsAttribute
                {
                    MultipartBodyLengthLimit = maxFileUploadLimitInBytes,
                } // Max size per file
            );
    }
}
