using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VBaseProject.Api.Models;
using VBaseProject.Test.Configuration;
using Xunit;

namespace VBaseProject.Test.Controllers
{
    public class UserControllerTest : ApiTestInitializer
    {
        [Fact]
        public async Task RequestWhitoutAuthenticationUserTest()
        {
            var response = await _client.GetAsync("/api/Users");
         
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task LoginValidTest()
        {
            var user = new UserLoginModel
            {
                Email = "vanderlan.gs@gmail.com",
                Password = "123"
            };

            var response = await _client.PostAsJsonAsync("api/Auth/login", user);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
