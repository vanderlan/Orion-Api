using Microsoft.Extensions.DependencyInjection;
using VBaseProject.Data.UnitOfWork;
using VBaseProject.Service.DependenciesConfig;

namespace VBaseProject.Test.Configuration
{
    public class DependencyInjectionSetupFixture
    {
        public ServiceProvider ServiceProvider { get; private set; }

        public DependencyInjectionSetupFixture()
        {
            var serviceCollection = new ServiceCollection();

            DependenciesInjectionConfiguration.Configure(serviceCollection, true);

            serviceCollection.AddTransient<IUnitOfWorkEntity>(s => new UnitOfWorkEntity(TestBootstrapper.GetInMemoryDbContextOptions()));
            serviceCollection.AddLogging();
            serviceCollection.AddLocalization(options => options.ResourcesPath = @"Resources");

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
