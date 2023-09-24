using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Orion.Api;
using Orion.Api.Models;

namespace Orion.Test.Configuration
{
    public abstract class ApiTestInitializer : WebApplicationFactory<Startup>
    {
        protected HttpClient Client;
        protected string AuthToken;

        public ApiTestInitializer()
        {

        }

        public void Setup()
        {
            var builder = new WebHostBuilder();

            base.ConfigureWebHost(builder);

            Client = Server.CreateClient();

            AuthUser();

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
        }

        public void AuthUser()
        {
            var result = Client.PostAsync("/api/Auth/Login", GetStringContent(new UserLoginModel { Email = "vanderlan.gs@gmail.com", Password = "123" })).GetAwaiter().GetResult();

            var content = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            var tokenResult = JsonConvert.DeserializeObject<UserApiTokenModel>(content);

            AuthToken = tokenResult.Token;
        }

        protected static StringContent GetStringContent(object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test")
                .UseStartup<Startup>();

            base.ConfigureWebHost(builder);
        }
    }
}
