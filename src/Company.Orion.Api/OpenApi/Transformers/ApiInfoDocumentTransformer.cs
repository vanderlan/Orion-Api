using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Company.Orion.Api.OpenApi.Transformers;

/// <summary>
/// Sets the title/description of the generated document. The version comes from the document name
/// (one document per API version, see <c>OpenApiConfiguration</c>).
/// </summary>
public sealed class ApiInfoDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = "Orion API",
            Version = context.DocumentName,
            Description = "Orion API - A simple project template for creating a .NET API (v10.0). " +
                          "Authenticate on POST /Auth/Login and send the token as 'Authorization: Bearer {token}'. " +
                          "The API version is selected with the 'x-api-version' header (defaults to 1.0)."
        };

        return Task.CompletedTask;
    }
}
