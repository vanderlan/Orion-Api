using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;

namespace VBaseProject.Test.Configuration
{
    public abstract class ApiTestInitializer : WebApplicationFactory<Startup>
    {
        protected HttpClient _client;

        public ApiTestInitializer()
        {
            Setup();
        }

        public void Setup()
        {
            var builder = new WebHostBuilder();

            base.ConfigureWebHost(builder);

            _client = Server.CreateClient();
        }
       
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development")
                .UseStartup<Startup>();

            base.ConfigureWebHost(builder);
        }
    }
}
