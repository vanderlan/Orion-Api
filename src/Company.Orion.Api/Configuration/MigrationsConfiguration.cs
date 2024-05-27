using Microsoft.EntityFrameworkCore;
using Company.Orion.Infra.Data.Context;

namespace Company.Orion.Api.Configuration;

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
