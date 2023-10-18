using Asp.Versioning;

namespace Orion.Api.Configuration;

public static class ApiVersioningConfiguration
{
    public static void ConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1,0);
            options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
        }).AddMvc();
    }
}
