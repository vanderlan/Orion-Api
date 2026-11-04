using Company.Orion.Api.OpenApi.Examples;
using Company.Orion.Api.OpenApi.Transformers;
using Scalar.AspNetCore;

namespace Company.Orion.Api.Configuration;

public static class OpenApiConfiguration
{
    /// <summary>
    /// One OpenAPI document per API version. The names must match the ApiExplorer group names
    /// produced by <c>ApiVersioningConfiguration</c> ("'v'VVV" => v1, v2, v2.1...).
    /// Add a new entry here when a new [ApiVersion] is introduced.
    /// </summary>
    public static readonly string[] DocumentNames = ["v1"];

    public static void ConfigureOpenApi(this IServiceCollection services)
    {
        //Registers every example provider found in the API assembly.
        services.AddOpenApiExamples();

        foreach (var documentName in DocumentNames)
        {
            services.AddOpenApi(documentName, options =>
            {
                options.AddDocumentTransformer<ApiInfoDocumentTransformer>();
                options.AddDocumentTransformer<BearerSecuritySchemeDocumentTransformer>();
                options.AddOperationTransformer<BearerSecurityRequirementOperationTransformer>();
                options.AddOperationTransformer<ExamplesOperationTransformer>();
            });
        }
    }

    /// <summary>
    /// Exposes the documents on <c>/openapi/{documentName}.json</c> and the Scalar API reference on
    /// <c>/scalar/{documentName}</c> (e.g. /scalar/v1).
    /// </summary>
    public static void UseOpenApiDocumentation(this WebApplication app)
    {
        app.MapOpenApi();

        app.MapScalarApiReference(options => options.WithTitle("Orion API"));
    }
}
