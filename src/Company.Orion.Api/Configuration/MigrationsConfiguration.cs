#if (systemDatabase == SqlServer || systemDatabase == PostgreSql)
using Microsoft.EntityFrameworkCore;
using Company.Orion.Infra.Data.Context;
#else
using Company.Orion.Infra.Data.Mongo.Initialization;
#endif

namespace Company.Orion.Api.Configuration;

public static class MigrationsConfiguration
{
#if (systemDatabase == SqlServer || systemDatabase == PostgreSql)
    public static WebApplication RunMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        if (dataContext.Database.GetPendingMigrations().Any())
        {
            dataContext.Database.Migrate();
        }

        return app;
    }
#else
    /// <summary>
    /// MongoDb is schemaless and has no migrations, so the startup only guarantees
    /// that the indexes and the default data exist.
    /// </summary>
    public static WebApplication RunMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<MongoDbInitializer>();

        initializer.InitializeAsync().GetAwaiter().GetResult();

        return app;
    }
#endif
}
