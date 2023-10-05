using Microsoft.Extensions.DependencyInjection;
using Orion.Data.UnitOfWork;
using Orion.Domain.Implementation;
using Orion.Domain.Interfaces;
using Orion.Domain.Repositories.UnitOfWork;

namespace Orion.Ioc.Dependencies
{
    public static class DependenciesInjectionConfiguration
    {
        public static void AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IUserService, UserService>();
        }
    }
}
