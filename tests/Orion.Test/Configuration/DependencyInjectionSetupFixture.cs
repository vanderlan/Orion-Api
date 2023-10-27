using Microsoft.Extensions.DependencyInjection;
using Orion.Infra.Data.UnitOfWork;
using Orion.Core.Domain.Repositories.UnitOfWork;
using Orion.Croscutting.Ioc.Dependencies;

namespace Orion.Test.Configuration;

public class DependencyInjectionSetupFixture
{
    public ServiceProvider ServiceProvider { get; private set; }

    public DependencyInjectionSetupFixture()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDomainServices();

        serviceCollection.AddTransient<IUnitOfWork>(s => new UnitOfWork(TestBootstrapper.GetInMemoryDbContextOptions()));
        serviceCollection.AddLogging();
        serviceCollection.AddLocalization(options => options.ResourcesPath = @"Resources");

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }
}
