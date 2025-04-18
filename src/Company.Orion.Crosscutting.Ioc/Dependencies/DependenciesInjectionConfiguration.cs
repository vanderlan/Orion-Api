using Microsoft.Extensions.DependencyInjection;
using Company.Orion.Infra.Data.UnitOfWork;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using Company.Orion.Infra.Data.Context;

namespace Company.Orion.Crosscutting.Ioc.Dependencies;

public static class DependenciesInjectionConfiguration
{
    public static void AddDatabaseContext(this IServiceCollection services)
    {
        services.AddScoped<DataContext>();
    }

    public static void AddUnitOfWork(this IServiceCollection services)
    {
        services.AddTransient<IUnitOfWork, UnitOfWork>();
    }
}
