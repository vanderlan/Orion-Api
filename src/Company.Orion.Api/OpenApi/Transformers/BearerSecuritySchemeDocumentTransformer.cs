using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Company.Orion.Api.OpenApi.Transformers;

/// <summary>
/// Declares the JWT Bearer security scheme on the document so API reference UIs show the
/// "Authorize" action. Operations reference it through <see cref="BearerSecurityRequirementOperationTransformer"/>.
/// </summary>
public sealed class BearerSecuritySchemeDocumentTransformer : IOpenApiDocumentTransformer
{
    public const string SchemeName = "Bearer";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes[SchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT obtained on POST /Auth/Login. Inform only the token, the 'Bearer ' prefix is added automatically."
        };

        return Task.CompletedTask;
    }
}
