using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Orion.Api.Configuration;

public static class HealthCheckConfiguration
{
    public static void ConfigureHealthCheck(this IApplicationBuilder app) 
    {
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
    }

    public static void AddApplicationHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks().AddSqlServer(configuration["ConnectionStrings:OrionDatabase"],
            tags: new[] 
            { 
                "database"
            });

        services.AddHealthChecks().AddElasticsearch(configuration["ElasticConfiguration:Uri"],
            tags: new[] 
            { 
                "elasticsearch", 
                "kibana" 
            });
    }
}
