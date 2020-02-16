using Microsoft.Extensions.DependencyInjection;
using VBaseProject.Data.UnitOfWork;
using VBaseProject.Service.Implementation;
using VBaseProject.Service.Interfaces;

namespace VBaseProject.Service.DependenciesConfig
{
    public class DependenciesInjectionConfiguration
    {
        public static void Configure(IServiceCollection services, bool isTest = false)
        {
            //IF TEST the UnitOfWork are configure to use InMemory Database
            if (!isTest)
            {
                services.AddScoped<IUnitOfWorkEntity, UnitOfWorkEntity>();
            }

            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IUserService, UserService>();
        }
    }
}
