using Microsoft.Extensions.DependencyInjection;
using Orion.Infra.Data.UnitOfWork;
using Orion.Core.Domain.Services.Interfaces;
using Orion.Core.Domain.Repositories.UnitOfWork;
using Orion.Core.Domain.Services;
using Orion.Infra.Data.Context;

namespace Orion.Croscutting.Ioc.Dependencies;

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
