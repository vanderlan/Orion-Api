using Orion.Application.Core.Commands.UserCreate;
using Orion.Test.Api.Setup;
using Orion.Test.Configuration.Faker;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Orion.Test.Api
{
    public class UsersApiTest : ApiTestsBootstrapper
    {
        public UsersApiTest(ApiTestsFixture fixture) : base(fixture)
        {
            AuthUser();
        }

        [Fact]
        public async Task PostUser_WithValidData_CreateAUser()
        {
            //arrange
            var request = UserFaker.GetUserCreateRequest();

            //act
            var httpResponse = await AuthenticatedHttpClient.PostAsync("/api/Users", GetStringContent(request));

            var userCreated = await GetResultContentAsync<UserCreateResponse>(httpResponse);

            //assert
            Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);

            Assert.NotNull(userCreated);
            Assert.Equal(request.Email, userCreated.Email);
            Assert.Equal(request.Name, userCreated.Name);
            Assert.Equal(request.Profile, userCreated.Profile);
            Assert.True(userCreated.CreatedAt > DateTime.MinValue);
            Assert.True(userCreated.LastUpdated > DateTime.MinValue);
            Assert.NotNull(userCreated.PublicId);
        }

        [Fact]
        public async Task PutUser_WithValidData_CreateAUser()
        {
            //arrange
            var userCreated = await CreateUserAsync();

            var userUpdateRequest = UserFaker.GetUserUpdateRequest();
            userUpdateRequest.PublicId = userCreated.PublicId;

            //act
            var httpResponsePut = await AuthenticatedHttpClient.PutAsync($"/api/Users/{userCreated.PublicId}", GetStringContent(userUpdateRequest));

            Assert.Equal(HttpStatusCode.Accepted, httpResponsePut.StatusCode);

            var userGetHttpResponse = await AuthenticatedHttpClient.GetAsync($"/api/Users/{userCreated.PublicId}");

            var user = await GetResultContentAsync<UserCreateResponse>(userGetHttpResponse);

            //assert
            Assert.NotNull(user);
            Assert.Equal(userUpdateRequest.Email, user.Email);
            Assert.Equal(userUpdateRequest.Name, user.Name);
            Assert.Equal(userUpdateRequest.Profile, user.Profile);
            Assert.True(user.LastUpdated > userCreated.LastUpdated);
            Assert.NotNull(user.PublicId);
        }

        [Fact]
        public async Task DeleteUser_WithValidIdData_DeleteTheUser()
        {
            //arrange
            var userCreated = await CreateUserAsync();

            //act
            var httpResponsePut = await AuthenticatedHttpClient.DeleteAsync($"/api/Users/{userCreated.PublicId}");

            Assert.Equal(HttpStatusCode.NoContent, httpResponsePut.StatusCode);

            var userGetHttpResponse = await AuthenticatedHttpClient.GetAsync($"/api/Users/{userCreated.PublicId}");

            //assert
            Assert.Equal(HttpStatusCode.NotFound, userGetHttpResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_WithInvalidId_ReturnsNotFound()
        {
            //arrange & act
            var httpResponseDelete = await AuthenticatedHttpClient.DeleteAsync($"/api/Users/{Guid.NewGuid()}");

            //assert
            Assert.Equal(HttpStatusCode.NotFound, httpResponseDelete.StatusCode);
        }

        private async Task<UserCreateResponse> CreateUserAsync()
        {
            var createUserRequest = UserFaker.GetUserCreateRequest();

            var httpResponsePost = await AuthenticatedHttpClient.PostAsync("/api/Users", GetStringContent(createUserRequest));

            Assert.Equal(HttpStatusCode.Created, httpResponsePost.StatusCode);

            return await GetResultContentAsync<UserCreateResponse>(httpResponsePost);
        }
    }
}
