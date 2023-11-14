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
using System.Threading.Tasks;

namespace Orion.Test.Api.Setup
{
    public class ApiTestsFixture
    {
        public readonly HttpClient HttpClient;
        public readonly HttpClient AuthenticatedHttpClient;
        public readonly User DefaultSystemUser;
        private readonly IUnitOfWork _unitOfWork;
        private IConfiguration _configuration;
        public IServiceProvider ServiceProvider { get; private set; }

        public ApiTestsFixture()
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
            BeforeEachTest().GetAwaiter().GetResult();
        }

        private async Task BeforeEachTest()
        {
            var tablesToTruncate = new[] { "User", "RefreshToken" };

            await using SqlConnection connection = new(_configuration["ConnectionStrings:OrionDatabase"]);
            connection.Open();

            foreach (var table in tablesToTruncate)
            {
                var command = new SqlCommand($"TRUNCATE TABLE dbo.[{table}]", connection);

                command.ExecuteNonQuery();
            }

            await _unitOfWork.UserRepository.AddAsync(DefaultSystemUser);
            await _unitOfWork.CommitAsync();
        }
    }
}
