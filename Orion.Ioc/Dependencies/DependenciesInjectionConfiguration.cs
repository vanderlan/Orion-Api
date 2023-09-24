using Microsoft.Extensions.DependencyInjection;
using Orion.Data.UnitOfWork;
using Orion.Domain.Implementation;
using Orion.Domain.Interfaces;
using Orion.Domain.Repositories.UnitOfWork;

namespace Orion.Ioc.Dependencies
{
    public static class DependenciesInjectionConfiguration
    {
        public static void AddDomainServices(this IServiceCollection services, bool isTest = false)
        {
            AddUnitOfWork(services, isTest);

            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IUserService, UserService>();
        }

        private static void AddUnitOfWork(IServiceCollection services, bool isTest)
        {
            //IF TEST the UnitOfWork are configured to use InMemory Database
            if (!isTest)
                services.AddScoped<IUnitOfWorkEntity, UnitOfWorkEntity>();
        }
    }
}
