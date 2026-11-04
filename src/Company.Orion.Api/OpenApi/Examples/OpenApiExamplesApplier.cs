using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace Company.Orion.Api.OpenApi.Examples;

/// <summary>
/// Fills the request body and the response examples of an operation using the example providers
/// registered for the types handled by the operation (<see cref="IExampleProvider{T}"/>,
/// <see cref="IMultipleExamplesProvider{T}"/> and <see cref="IResponseExampleProvider{T}"/>).
/// Examples are serialized with the same JSON options used by the controllers, so what is shown in
/// the API reference is exactly what the API accepts and returns.
/// </summary>
public sealed class OpenApiExamplesApplier(
    IServiceProvider serviceProvider,
    IOptions<Microsoft.AspNetCore.Mvc.JsonOptions> jsonOptions)
{
    private const string DefaultExampleName = "default";

    private readonly JsonSerializerOptions _serializerOptions = jsonOptions.Value.JsonSerializerOptions;

    public void Apply(OpenApiOperation operation, ApiDescription apiDescription)
    {
        ApplyRequestExamples(operation, apiDescription);
        ApplyResponseExamples(operation, apiDescription);
    }

    private void ApplyRequestExamples(OpenApiOperation operation, ApiDescription apiDescription)
    {
        var bodyParameter = apiDescription.ParameterDescriptions
            .FirstOrDefault(parameter => parameter.Source == BindingSource.Body);

        if (bodyParameter?.Type is null || operation.RequestBody?.Content is null)
            return;

        ApplyExamples(operation.RequestBody.Content, ResolveExamples(bodyParameter.Type));
    }

    private void ApplyResponseExamples(OpenApiOperation operation, ApiDescription apiDescription)
    {
        if (operation.Responses is null)
            return;

        foreach (var responseType in apiDescription.SupportedResponseTypes)
        {
            if (responseType.IsDefaultResponse || responseType.Type is null || responseType.Type == typeof(void))
                continue;

            if (!operation.Responses.TryGetValue(responseType.StatusCode.ToString(), out var response) || response?.Content is null)
                continue;

            ApplyExamples(response.Content, ResolveExamples(responseType.Type, responseType.StatusCode));
        }
    }

    private void ApplyExamples(IDictionary<string, OpenApiMediaType> content, List<INamedExample> examples)
    {
        if (examples.Count == 0)
            return;

        foreach (var mediaType in content.Values)
        {
            if (examples.Count == 1)
            {
                mediaType.Example = ToJsonNode(examples[0].Value);
                continue;
            }

            mediaType.Examples = examples.ToDictionary(
                example => example.Name,
                example => (IOpenApiExample)new OpenApiExample
                {
                    Summary = example.Summary,
                    Description = example.Description,
                    Value = ToJsonNode(example.Value)
                });
        }
    }

    /// <summary>
    /// Resolution order: status-aware response provider (responses only), single example provider,
    /// multiple examples provider.
    /// </summary>
    private List<INamedExample> ResolveExamples(Type type, int? statusCode = null)
    {
        var examples = new List<INamedExample>();

        if (statusCode.HasValue
            && serviceProvider.GetService(typeof(IResponseExampleProvider<>).MakeGenericType(type)) is IResponseExampleProvider responseProvider)
        {
            examples.Add(new NamedExample<object>(DefaultExampleName, responseProvider.GetExampleValue(statusCode.Value)));
        }
        else if (serviceProvider.GetService(typeof(IExampleProvider<>).MakeGenericType(type)) is IExampleProvider singleProvider)
        {
            examples.Add(new NamedExample<object>(DefaultExampleName, singleProvider.GetExampleValue()));
        }
        else if (serviceProvider.GetService(typeof(IMultipleExamplesProvider<>).MakeGenericType(type)) is IMultipleExamplesProvider multipleProvider)
        {
            examples.AddRange(multipleProvider.GetNamedExamples());
        }

        examples.RemoveAll(example => example?.Value is null);

        return examples;
    }

    private JsonNode ToJsonNode(object value)
    {
        return JsonSerializer.SerializeToNode(value, value.GetType(), _serializerOptions);
    }
}
