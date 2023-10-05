using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Orion.Data.UnitOfWork;
using Orion.Domain.Repositories.UnitOfWork;
using Orion.Ioc.Dependencies;

namespace Orion.Test.Configuration
{
    public class DependencyInjectionSetupFixture
    {
        public ServiceProvider ServiceProvider { get; private set; }

        public DependencyInjectionSetupFixture()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDomainServices();

            serviceCollection.RemoveAll<IUnitOfWork>();

            serviceCollection.AddTransient<IUnitOfWork>(s => new UnitOfWork(TestBootstrapper.GetInMemoryDbContextOptions()));
            serviceCollection.AddLogging();
            serviceCollection.AddLocalization(options => options.ResourcesPath = @"Resources");

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
