using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using System;
using System.Net.Http;
using Company.Orion.Test.Shared.Faker;

namespace Company.Orion.Test.Integration.Setup
{
    public class IntegrationTestsFixture
    {
        public readonly HttpClient HttpClient;
        public readonly HttpClient AuthenticatedHttpClient;
        public readonly User DefaultSystemUser;
        public readonly IServiceProvider ServiceProvider;
        private readonly IUnitOfWork _unitOfWork;
        private SqlConnection _sqlConnection;
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

                                    SetupDatabase(config);

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

        private void SetupDatabase(IConfiguration config)
        {
            const string connectionPath = "ConnectionStrings:OrionDatabase";

            _databaseName = $"OrionTests{DateTime.UtcNow.Ticks}";

            _sqlConnection = new SqlConnection(config[connectionPath]);

            CreateDatabase(_databaseName);

            _connectionString = config[connectionPath].Replace("OrionTests", _databaseName);
            config[connectionPath] = _connectionString;

            _sqlConnection = new SqlConnection(_connectionString);
        }

        public HttpClient GetNewHttpClient()
        {
            return _appFactory.CreateClient();
        }

        private void CreateDatabase(string databaseName)
        {
            lock (_sqlConnection)
            {
                using (_sqlConnection)
                {
                    _sqlConnection.Open();
                    
                    var command = new SqlCommand($"CREATE DATABASE {databaseName}", _sqlConnection);

                    command.ExecuteNonQuery();
                    _sqlConnection.Close();
                }
            }
        }
    }
}
