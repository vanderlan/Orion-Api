using Microsoft.Extensions.DependencyInjection;
using Orion.Data.UnitOfWork;
using Orion.Domain.Services.Interfaces;
using Orion.Domain.Repositories.UnitOfWork;
using Orion.Domain.Services;
using Orion.Data.Context;

namespace Orion.Ioc.Dependencies;

public static class DependenciesInjectionConfiguration
{
    public static void AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
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
