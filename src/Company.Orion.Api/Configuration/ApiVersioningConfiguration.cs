using Asp.Versioning;

namespace Company.Orion.Api.Configuration;

public static class ApiVersioningConfiguration
{
    public static void ConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
        })
        .AddMvc()
        .AddApiExplorer(options =>
        {
            //Groups the endpoints by version so each OpenAPI document ("v1", "v2"...) only
            //contains the operations of its version and documents the x-api-version header.
            options.GroupNameFormat = "'v'VVV";
        });
    }
}
