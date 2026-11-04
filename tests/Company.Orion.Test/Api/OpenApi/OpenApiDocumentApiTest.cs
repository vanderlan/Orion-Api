using System.Net;
using System.Threading.Tasks;
using Company.Orion.Test.Integration.Setup;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Company.Orion.Test.Api.OpenApi;

/// <summary>
/// Guarantees that the generated OpenAPI document keeps exposing the request/response examples and
/// the security metadata, which are part of the developer experience of the API.
/// </summary>
public class OpenApiDocumentApiTest(IntegrationTestsFixture fixture) : IntegrationTestsBootstrapper(fixture)
{
    private const string DocumentPath = "/openapi/v1.json";

    [Fact]
    public async Task GetOpenApiDocument_ReturnsOkWithRequestAndResponseExamples()
    {
        //arrange & act
        var httpResponse = await HttpClient.GetAsync(DocumentPath);
        var document = JObject.Parse(await httpResponse.Content.ReadAsStringAsync());

        //assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal("Orion API", document.SelectToken("info.title")?.Value<string>());

        //single example on request and response
        Assert.Equal("adm@orion.com",
            document.SelectToken("paths['/Auth/Login'].post.requestBody.content['application/json'].example.email")?.Value<string>());
        Assert.NotNull(document.SelectToken("paths['/Auth/Login'].post.responses['200'].content['application/json'].example.token"));

        //multiple named examples
        Assert.NotNull(document.SelectToken("paths['/Users'].post.requestBody.content['application/json'].examples.admin.value.email"));
        Assert.NotNull(document.SelectToken("paths['/Users'].post.requestBody.content['application/json'].examples.operator.value.email"));

        //status-aware ProblemDetails example
        var notFoundExample =
            document.SelectToken("paths['/Users/{id}'].get.responses['404'].content['application/problem+json'].example")
            ?? document.SelectToken("paths['/Users/{id}'].get.responses['404'].content['application/json'].example");

        Assert.NotNull(notFoundExample);
        Assert.Equal(404, notFoundExample.Value<int>("status"));
    }

    [Fact]
    public async Task GetOpenApiDocument_DeclaresBearerSecurityOnlyForProtectedOperations()
    {
        //arrange & act
        var httpResponse = await HttpClient.GetAsync(DocumentPath);
        var document = JObject.Parse(await httpResponse.Content.ReadAsStringAsync());

        //assert
        Assert.Equal("http", document.SelectToken("components.securitySchemes.Bearer.type")?.Value<string>());
        Assert.Equal("bearer", document.SelectToken("components.securitySchemes.Bearer.scheme")?.Value<string>());

        Assert.NotNull(document.SelectToken("paths['/Users'].get.security[0].Bearer"));
        Assert.Null(document.SelectToken("paths['/Auth/Login'].post.security"));
    }
}
