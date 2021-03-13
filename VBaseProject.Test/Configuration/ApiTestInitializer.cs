using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using VBaseProject.Api.Models;

namespace VBaseProject.Test.Configuration
{
    public abstract class ApiTestInitializer : WebApplicationFactory<Startup>
    {
        protected HttpClient _client;
        protected string AuthToken;

        public ApiTestInitializer()
        {
            Setup();
        }

        public void Setup()
        {
            var builder = new WebHostBuilder();

            base.ConfigureWebHost(builder);

            _client = Server.CreateClient();
            
            AuthUser();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
        }

        public void AuthUser()
        {
            var result = _client.PostAsync("/api/Auth/Login", GetStringContent(new UserLoginModel { Email = "vanderlan.gs@gmail.com", Password = "123" })).Result;

            var content = result.Content.ReadAsStringAsync().Result;

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
