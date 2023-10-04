using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Orion.Api.Models;
using System;
using System.Net.Http;
using System.Text;

namespace Orion.Test.Configuration
{
    public abstract class ApiTestInitializer : IDisposable
    {
        protected string AuthToken;
                protected readonly HttpClient Client;
        protected readonly HttpClient AuthenticatedClient;
        protected IServiceProvider ServiceProvider { get; private set; }

        public ApiTestInitializer()
        {
            var appFactory = new WebApplicationFactory<Program>()
               .WithWebHostBuilder(builder =>
               {
                   var config = new ConfigurationBuilder()
                       .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
                       .Build();

                   builder
                       .UseConfiguration(config)
                       .ConfigureServices(services =>
                       {
                       });
               });
            ServiceProvider = appFactory.Services;
            Client = appFactory.CreateClient();
            AuthenticatedClient = appFactory.CreateClient();
        }

        public void AuthUser()
        {
            var result = Client.PostAsync("/api/Auth/Login", GetStringContent(
                new UserLoginModel { 
                    Email = "vanderlan.gs@gmail.com", 
                    Password = "123" 
                }))
                .GetAwaiter().GetResult();

            var content = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            var tokenResult = JsonConvert.DeserializeObject<UserApiTokenModel>(content);

            AuthToken = tokenResult.Token;
        }

        protected static StringContent GetStringContent(object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");
        }

        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
               _disposedValue = true;
            }
        }

        ~ApiTestInitializer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
