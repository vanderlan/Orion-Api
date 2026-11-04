using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Company.Orion.Api.Configuration;

public static class HealthCheckConfiguration
{
    public const string BasePath = "/health";
    public const string LivenessPath = "/health/live";
    public const string ReadinessPath = "/health/ready";

    private const string ReadinessTag = "ready";

    private static readonly string[] DatabaseTags =
            [
                "database",
                ReadinessTag
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

    /// <summary>
    /// Kubernetes/Docker style probes:
    /// <list type="bullet">
    /// <item><c>/health/live</c> — the process is up (no dependency is checked). Use it as liveness probe: a failure restarts the container.</item>
    /// <item><c>/health/ready</c> — the dependencies tagged "ready" (database) are reachable. Use it as readiness probe / load balancer check: a failure stops the traffic, not the container.</item>
    /// <item><c>/health</c> — the full report of every check, in the HealthChecks UI format.</item>
    /// </list>
    /// </summary>
    public static void ConfigureHealthCheck(this WebApplication app)
    {
        app.MapHealthChecks(LivenessPath, new HealthCheckOptions
        {
            Predicate = _ => false
        });

        app.MapHealthChecks(ReadinessPath, new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(ReadinessTag),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecks(BasePath, new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
    }
}
