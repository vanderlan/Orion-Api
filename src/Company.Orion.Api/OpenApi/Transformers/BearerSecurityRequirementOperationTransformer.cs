using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Company.Orion.Api.OpenApi.Transformers;

/// <summary>
/// Adds the Bearer security requirement to every operation protected by <c>[Authorize]</c>
/// (or derived attributes such as <c>[AuthorizeFor]</c>) and not marked with <c>[AllowAnonymous]</c>,
/// so the API reference sends the token only where it is needed.
/// </summary>
public sealed class BearerSecurityRequirementOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;

        var requiresAuthorization = metadata.OfType<IAuthorizeData>().Any()
                                    && !metadata.OfType<IAllowAnonymous>().Any();

        if (!requiresAuthorization)
            return Task.CompletedTask;

        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(BearerSecuritySchemeDocumentTransformer.SchemeName, context.Document)] = []
        });

        return Task.CompletedTask;
    }
}
