using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Company.Orion.Api.Configuration;

public static class HealthCheckConfiguration
{
    private static readonly string[] DatabaseTags =
            [
                "database"
            ];

    public static void AddApplicationHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {

#if (systemDatabase == SqlServer)
        services.AddHealthChecks().AddSqlServer(configuration["ConnectionStrings:SqlServer"],
            tags: DatabaseTags);
#elif (systemDatabase == PostgreSql)
        services.AddHealthChecks().AddNpgSql(configuration["ConnectionStrings:PostgreSql"], 
            tags: DatabaseTags);
#else
        //The MongoDb health check uses the IMongoClient registered by AddDatabaseContext.
        services.AddHealthChecks().AddMongoDb(tags: DatabaseTags);
#endif

    }

    public static void ConfigureHealthCheck(this IApplicationBuilder app) 
    {
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
    }
}
