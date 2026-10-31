using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using System;
using System.Net.Http;
using Company.Orion.Test.Shared.Faker;
#if (systemDatabase == SqlServer)
using Microsoft.Data.SqlClient;
#elif (systemDatabase == PostgreSql)
using Npgsql;
#endif

namespace Company.Orion.Test.Integration.Setup
{
    public class IntegrationTestsFixture
    {
        public readonly HttpClient HttpClient;
        public readonly HttpClient AuthenticatedHttpClient;
        public readonly User DefaultSystemUser;
        public readonly IServiceProvider ServiceProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly WebApplicationFactory<Program> _appFactory;
        private string _databaseName;
        private string _connectionString;

        public IntegrationTestsFixture()
        {
            var appFactory = new WebApplicationFactory<Program>()
                                .WithWebHostBuilder(builder =>
                                {
                                    var config = new ConfigurationBuilder()
                                        .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
                                        .Build();

                                #if (systemDatabase == SqlServer)
                                    SetupSqlServerDatabase(config);
                                #elif (systemDatabase == PostgreSql)
                                    SetupPostgreSqlDatabase(config);
                                #else
                                    SetupMongoDbDatabase(config);
                                #endif

                                    builder.UseConfiguration(config);
                                });

            ServiceProvider = appFactory.Services;

            _unitOfWork = appFactory.Services.GetRequiredService<IUnitOfWork>();

            HttpClient = appFactory.CreateClient();
            AuthenticatedHttpClient = appFactory.CreateClient();

            DefaultSystemUser = UserFaker.GetDefaultSystemUser();

            _appFactory = appFactory;

            _unitOfWork.UserRepository.AddAsync(DefaultSystemUser).GetAwaiter().GetResult();
            _unitOfWork.CommitAsync().GetAwaiter().GetResult();
        }

#if (systemDatabase == SqlServer)
        private void SetupSqlServerDatabase(IConfiguration config)
        {
            const string connectionPath = "ConnectionStrings:SqlServer";

            _databaseName = $"OrionTests{DateTime.UtcNow.Ticks}";

            var baseConnectionString = TestDatabaseContainer.ConnectionString;

            var sqlServerConnection = new SqlConnection(baseConnectionString);

            lock (sqlServerConnection)
            {
                using (sqlServerConnection)
                {
                    sqlServerConnection.Open();

                    var command = new SqlCommand($"CREATE DATABASE {_databaseName}", sqlServerConnection);

                    command.ExecuteNonQuery();
                    sqlServerConnection.Close();
                }
            }

            _connectionString = new SqlConnectionStringBuilder(baseConnectionString) { InitialCatalog = _databaseName }.ConnectionString;
            config[connectionPath] = _connectionString;
        }
#elif (systemDatabase == PostgreSql)
        private void SetupPostgreSqlDatabase(IConfiguration config)
        {
            const string connectionPath = "ConnectionStrings:PostgreSql";

            _databaseName = $"OrionTests{DateTime.UtcNow.Ticks}";

            var baseConnectionString = TestDatabaseContainer.ConnectionString;

            var postgreSqlConnection = new NpgsqlConnection(baseConnectionString);

            lock (postgreSqlConnection)
            {
                using (postgreSqlConnection)
                {
                    postgreSqlConnection.Open();

                    var command = new NpgsqlCommand($"CREATE DATABASE {_databaseName}", postgreSqlConnection);

                    command.ExecuteNonQuery();
                    postgreSqlConnection.Close();
                }
            }

            _connectionString = new NpgsqlConnectionStringBuilder(baseConnectionString) { Database = _databaseName }.ConnectionString;
            config[connectionPath] = _connectionString;
        }
#else
        private void SetupMongoDbDatabase(IConfiguration config)
        {
            const string connectionPath = "ConnectionStrings:MongoDb";
            const string databaseNamePath = "MongoDb:DatabaseName";

            //Each fixture runs on its own database of the shared MongoDb container.
            _databaseName = $"OrionTests{DateTime.UtcNow.Ticks}";

            _connectionString = TestDatabaseContainer.ConnectionString;

            config[connectionPath] = _connectionString;
            config[databaseNamePath] = _databaseName;
        }
#endif

        public HttpClient GetNewHttpClient()
        {
            return _appFactory.CreateClient();
        }
    }
}
