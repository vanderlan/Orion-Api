using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Company.Orion.Api.Configuration;

public static class HealthCheckConfiguration
{
    private static readonly string[] DatabaseTags =
            [
                "database"
            ];
    private static readonly string[] ElasticSearchTags =
            [
                "elasticsearch", 
                "kibana" 
            ];

    public static void AddApplicationHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks().AddSqlServer(configuration["ConnectionStrings:OrionDatabase"] ?? string.Empty,
            tags: DatabaseTags);

        services.AddHealthChecks().AddElasticsearch(configuration["ElasticConfiguration:Uri"],
            tags: ElasticSearchTags);
    }

    public static void ConfigureHealthCheck(this IApplicationBuilder app) 
    {
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
    }
}
