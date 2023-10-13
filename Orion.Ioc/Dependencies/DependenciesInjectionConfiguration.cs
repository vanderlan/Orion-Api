using Microsoft.Extensions.DependencyInjection;
using Orion.Data.UnitOfWork;
using Orion.Domain.Services.Interfaces;
using Orion.Domain.Repositories.UnitOfWork;
using Orion.Domain.Services;

namespace Orion.Ioc.Dependencies
{
    public static class DependenciesInjectionConfiguration
    {
        public static void AddDomainServices(this IServiceCollection services)
        {
            //Unit of Work
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            //Domain Services
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IUserService, UserService>();
        }
    }
}
