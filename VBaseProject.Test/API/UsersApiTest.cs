using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using VBaseProject.Api.AutoMapper.Output;
using VBaseProject.Test.Configuration;
using VBaseProject.Test.MotherObjects;
using Xunit;

namespace VBaseProject.Test.API
{
    public class UsersApiTest : ApiTestInitializer
    {
        [Fact]
        public async Task CreateUserSuccessTest()
        {
            var userInput = UserMotherObject.ValidAdminUserInput();

            userInput.Email = $"{Guid.NewGuid()}@vbaseproject.com";

            var result = await _client.PostAsync("/api/Users", GetStringContent(userInput));

            var content = await result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            
            var userCreated = JsonConvert.DeserializeObject<UserOutput>(content);

            Assert.Equal(userInput.Email, userCreated.Email);
        }
    }
}
