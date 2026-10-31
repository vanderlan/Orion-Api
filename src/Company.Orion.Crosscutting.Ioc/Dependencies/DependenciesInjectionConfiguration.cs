#if (systemDatabase == SqlServer || systemDatabase == PostgreSql)
using Microsoft.Extensions.DependencyInjection;
using Company.Orion.Infra.Data.UnitOfWork;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using Company.Orion.Infra.Data.Context;
#else
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    public static void AddDatabaseContext(this IServiceCollection services)
    {
        services.AddScoped<DataContext>();
    }

    public static void AddUnitOfWork(this IServiceCollection services)
    {
        services.AddTransient<IUnitOfWork, UnitOfWork>();
    }
#else
    public static void AddDatabaseContext(this IServiceCollection services)
    {
        services.AddSingleton(serviceProvider =>
            MongoDbOptions.FromConfiguration(serviceProvider.GetRequiredService<IConfiguration>()));

        //The MongoClient handles the connection pool and is designed to be shared by the whole application.
        services.AddSingleton<IMongoClient>(serviceProvider =>
            new MongoClient(serviceProvider.GetRequiredService<MongoDbOptions>().ConnectionString));

        services.AddScoped<MongoDataContext>();
        services.AddScoped<MongoDbInitializer>();
    }

    public static void AddUnitOfWork(this IServiceCollection services)
    {
        services.AddTransient<IUnitOfWork, MongoUnitOfWork>();
    }
#endif
}
