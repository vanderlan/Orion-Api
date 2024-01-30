using Microsoft.Extensions.DependencyInjection;
using Orion.Infra.Data.UnitOfWork;
using Orion.Domain.Core.Services.Interfaces;
using Orion.Domain.Core.Repositories.UnitOfWork;
using Orion.Domain.Core.Services;
using Orion.Infra.Data.Context;

namespace Orion.Crosscutting.Ioc.Dependencies;

public static class DependenciesInjectionConfiguration
{
    public static void AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
    }

    public static void AddDatabaseContext(this IServiceCollection services)
    {
        services.AddScoped<DataContext>();
    }

    public static void AddUnitOfWork(this IServiceCollection services)
    {
        services.AddTransient<IUnitOfWork, UnitOfWork>();
    }
}
