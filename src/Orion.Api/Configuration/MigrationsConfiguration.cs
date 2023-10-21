using Microsoft.EntityFrameworkCore;
using Orion.Data.Context;

namespace Orion.Api.Configuration;

public static class MigrationsConfiguration
{
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
}
