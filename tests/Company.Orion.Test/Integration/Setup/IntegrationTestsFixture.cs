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
#else
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
                                #else
                                    SetupPostgreSqlDatabase(config);
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

            var sqlServerConnection = new SqlConnection(config[connectionPath]);

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

            _connectionString = config[connectionPath].Replace("OrionTests", _databaseName);
            config[connectionPath] = _connectionString;
        }
#else
        private void SetupPostgreSqlDatabase(IConfiguration config)
        {
            const string connectionPath = "ConnectionStrings:PostgreSql";

            _databaseName = $"OrionTests{DateTime.UtcNow.Ticks}";

            var postgreSqlConnection = new NpgsqlConnection(config[connectionPath]);

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

            _connectionString = config[connectionPath].Replace("OrionTests", _databaseName);
            config[connectionPath] = _connectionString;
        }
#endif

        public HttpClient GetNewHttpClient()
        {
            return _appFactory.CreateClient();
        }
    }
}
