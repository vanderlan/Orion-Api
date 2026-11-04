using Company.Orion.Api.OpenApi.Examples;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Company.Orion.Api.OpenApi.Transformers;

/// <summary>
/// Thin adapter between the OpenAPI pipeline and the <see cref="OpenApiExamplesApplier"/>.
/// </summary>
public sealed class ExamplesOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        context.ApplicationServices
            .GetRequiredService<OpenApiExamplesApplier>()
            .Apply(operation, context.Description);

        return Task.CompletedTask;
    }
}
