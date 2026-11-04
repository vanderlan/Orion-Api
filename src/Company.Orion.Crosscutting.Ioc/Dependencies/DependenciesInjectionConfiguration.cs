#if (systemDatabase == SqlServer || systemDatabase == PostgreSql)
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using Company.Orion.Infra.Data.Context;
using Company.Orion.Infra.Data.Interceptors;
using Company.Orion.Infra.Data.UnitOfWork;
#else
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using Company.Orion.Infra.Data.Mongo.Configuration;
using Company.Orion.Infra.Data.Mongo.Context;
using Company.Orion.Infra.Data.Mongo.Initialization;
using Company.Orion.Infra.Data.Mongo.UnitOfWork;
using MongoDB.Driver;
#endif

namespace Company.Orion.Crosscutting.Ioc.Dependencies;

public static class DependenciesInjectionConfiguration
{
#if (systemDatabase == SqlServer || systemDatabase == PostgreSql)
    /// <summary>
    /// Registers a pooled, scoped <see cref="DataContext"/> configured from <c>ConnectionStrings</c>.
    /// The context receives its options from DI, so it is also available at design time (dotnet ef).
    /// </summary>
    public static void AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<TimestampsInterceptor>();

        services.AddDbContextPool<DataContext>((serviceProvider, options) =>
        {
#if (systemDatabase == SqlServer)
            options.UseSqlServer(configuration.GetSection("ConnectionStrings:SqlServer").Value);
#else
            options.UseNpgsql(configuration.GetSection("ConnectionStrings:PostgreSql").Value);
#endif
            options.AddInterceptors(serviceProvider.GetRequiredService<TimestampsInterceptor>());
        });
    }

    /// <summary>One unit of work per request, sharing the scoped context with every repository.</summary>
    public static void AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
#else
    public static void AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton(TimeProvider.System);

        services.AddSingleton(MongoDbOptions.FromConfiguration(configuration));

        //The MongoClient handles the connection pool and is designed to be shared by the whole application.
        services.AddSingleton<IMongoClient>(serviceProvider =>
            new MongoClient(serviceProvider.GetRequiredService<MongoDbOptions>().ConnectionString));

        services.AddScoped<MongoDataContext>();
        services.AddScoped<MongoDbInitializer>();
    }

    /// <summary>One unit of work per request, sharing the scoped context with every repository.</summary>
    public static void AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, MongoUnitOfWork>();
    }
#endif
}
