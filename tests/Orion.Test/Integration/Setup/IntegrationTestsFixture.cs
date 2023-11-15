using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Repositories.UnitOfWork;
using Orion.Test.Configuration.Faker;
using System;
using System.Net.Http;

namespace Orion.Test.Integration.Setup
{
    public class IntegrationTestsFixture
    {
        public readonly HttpClient HttpClient;
        public readonly HttpClient AuthenticatedHttpClient;
        public readonly User DefaultSystemUser;
        public readonly IServiceProvider ServiceProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SqlConnection _sqlConnection;
        private IConfiguration _configuration;
        private WebApplicationFactory<Program> AppFactory;

        public IntegrationTestsFixture()
        {
            var appFactory = new WebApplicationFactory<Program>()
                                .WithWebHostBuilder(builder =>
                                {
                                    var config = new ConfigurationBuilder()
                                        .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
                                        .Build();

                                    builder.UseConfiguration(config);

                                    _configuration = config;
                                });

            ServiceProvider = appFactory.Services;

            _unitOfWork = appFactory.Services.GetRequiredService<IUnitOfWork>();

            HttpClient = appFactory.CreateClient();
            AuthenticatedHttpClient = appFactory.CreateClient();

            DefaultSystemUser = UserFaker.GetDefaultSystemUser();

            _sqlConnection = new SqlConnection(_configuration["ConnectionStrings:OrionDatabase"]);

            AppFactory = appFactory;
            
            BeforeEachTest();
        }

        public HttpClient GetNewHttpClient()
        {
            return AppFactory.CreateClient();
        }
        
        private void BeforeEachTest()
        {
            var tablesToTruncate = new[] { "User", "RefreshToken" };

            lock (_sqlConnection)
            {
                using (_sqlConnection)
                {
                    _sqlConnection.Open();

                    foreach (var table in tablesToTruncate)
                    {
                        var command = new SqlCommand($"TRUNCATE TABLE dbo.[{table}]", _sqlConnection);

                        command.ExecuteNonQuery();
                    }

                    lock (_unitOfWork)
                    {
                        _unitOfWork.UserRepository.AddAsync(DefaultSystemUser).GetAwaiter().GetResult();
                        _unitOfWork.CommitAsync().GetAwaiter().GetResult();
                    }
                }
            }
        }
    }
}
