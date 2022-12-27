using Microsoft.Extensions.DependencyInjection;
using VBaseProject.Data.UnitOfWork;
using VBaseProject.Domain.Repositories.UnitOfWork;
using VBaseProject.Ioc.Dependencies;

namespace VBaseProject.Test.Configuration
{
    public class DependencyInjectionSetupFixture
    {
        public ServiceProvider ServiceProvider { get; private set; }

        public DependencyInjectionSetupFixture()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDomainServices(true);

            serviceCollection.AddTransient<IUnitOfWorkEntity>(s => new UnitOfWorkEntity(TestBootstrapper.GetInMemoryDbContextOptions()));
            serviceCollection.AddLogging();
            serviceCollection.AddLocalization(options => options.ResourcesPath = @"Resources");

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
