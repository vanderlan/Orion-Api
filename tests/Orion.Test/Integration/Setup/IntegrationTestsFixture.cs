using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orion.Croscutting.Ioc.Dependencies;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Repositories.UnitOfWork;
using Orion.Test.Configuration.Faker;
using System.Threading.Tasks;

namespace Orion.Test.Integration.Setup;

public class IntegrationTestsFixture
{
    public ServiceProvider ServiceProvider { get; private set; }
    public readonly IConfiguration configuration;
    public readonly User DefaultSystemUser;
    public readonly IUnitOfWork UnitOfWork;

    public IntegrationTestsFixture()
    {
        var config = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
                   .Build();

        configuration = config;

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDomainServices();
        serviceCollection.AddDatabaseContext();
        serviceCollection.AddUnitOfWork();
        serviceCollection.AddLogging();
        serviceCollection.AddLocalization(options => options.ResourcesPath = @"Resources");
        serviceCollection.AddSingleton<IConfiguration>(config);

        ServiceProvider = serviceCollection.BuildServiceProvider();

        UnitOfWork = ServiceProvider.GetRequiredService<IUnitOfWork>();

        DefaultSystemUser = UserFaker.GetDefaultSystemUser();
        BeforeEachTest().GetAwaiter().GetResult();
    }

    private async Task BeforeEachTest()
    {
        var tablesToTruncate = new[] { "User", "RefreshToken" };

        using SqlConnection connection = new(configuration["ConnectionStrings:OrionDatabase"]);
        connection.Open();

        foreach (var table in tablesToTruncate)
        {
            var command = new SqlCommand($"TRUNCATE TABLE dbo.[{table}]", connection);

            command.ExecuteNonQuery();
        }

        await UnitOfWork.UserRepository.AddAsync(DefaultSystemUser);
        await UnitOfWork.CommitAsync();
    }
}
