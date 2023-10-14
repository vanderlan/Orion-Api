using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Orion.Api.Configuration;

public static class ApiVersioningConfiguration
{
    public static void ConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(o =>
        {
            o.ReportApiVersions = true;
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.DefaultApiVersion = new ApiVersion(1, 0);
            o.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
        });
    }
}
