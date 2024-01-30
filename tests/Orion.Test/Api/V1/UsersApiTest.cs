using Orion.Application.Core.Commands.UserCreate;
using Orion.Test.Configuration.Faker;
using Orion.Test.Integration.Setup;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Orion.Application.Core.Commands.UserChangePassword;
using Orion.Application.Core.Queries.UserGetPaginated;
using Orion.Domain.Core.ValueObjects.Pagination;
using Xunit;

namespace Orion.Test.Api.V1
{
    public class UsersApiTest : IntegrationTestsBootstrapper
    {
        public UsersApiTest(IntegrationTestsFixture fixture) : base(fixture)
        {
            LoginWithDefaultUser();
        }
        
        [Fact]
        public async Task GetUserPaginated_WithoutFilter_ReturnsDefaultUser()
        {
            //arrange & act
            var getUsersHttpResponse = await AuthenticatedHttpClient.GetAsync("/api/Users");

            var listUsersPaginated = await GetResultContentAsync<PagedList<UserGetPaginatedResponse>>(getUsersHttpResponse);

            //assert
            Assert.Contains(listUsersPaginated.Items, x => x.PublicId == DefaultSystemUser.PublicId);
            Assert.Contains(listUsersPaginated.Items, x => x.Name == DefaultSystemUser.Name);
            Assert.Contains(listUsersPaginated.Items, x => x.Email == DefaultSystemUser.Email);
            Assert.True(listUsersPaginated.Count >= 1);
            Assert.Equal(HttpStatusCode.OK, getUsersHttpResponse.StatusCode);
        }
        
        [Fact]
        public async Task GetUserPaginated_WithNameFilter_ReturnsDefaultUser()
        {
            //arrange & act
            var getUsersHttpResponse = await AuthenticatedHttpClient.GetAsync($"/api/Users?filter.Query={DefaultSystemUser.Name}");

            var listUsersPaginated = await GetResultContentAsync<PagedList<UserGetPaginatedResponse>>(getUsersHttpResponse);

            //assert
            Assert.Contains(listUsersPaginated.Items, x => x.PublicId == DefaultSystemUser.PublicId);
            Assert.Contains(listUsersPaginated.Items, x => x.Name == DefaultSystemUser.Name);
            Assert.Contains(listUsersPaginated.Items, x => x.Email == DefaultSystemUser.Email);
            Assert.True(listUsersPaginated.Count >= 1);
            Assert.Equal(HttpStatusCode.OK, getUsersHttpResponse.StatusCode);
        }
        
        [Fact]
        public async Task GetUserPaginated_WithInvalidUserName_ReturnsEmptyList()
        {
            //arrange & act
            var getUsersHttpResponse = await AuthenticatedHttpClient.GetAsync($"/api/Users?filter.Query={Guid.NewGuid()}");

            var listUsersPaginated = await GetResultContentAsync<PagedList<UserGetPaginatedResponse>>(getUsersHttpResponse);

            //assert
            Assert.Empty(listUsersPaginated.Items);
            Assert.True(listUsersPaginated.Count == 0);
            Assert.Equal(HttpStatusCode.OK, getUsersHttpResponse.StatusCode);
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
        public async Task PutUser_WithValidData_UpdateUser()
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

        [Theory]
        [InlineData("123", "12345", "30298302")]
        [InlineData("", "12345", "30298302")]
        [InlineData("", "12345", null)]
        [InlineData("", "", null)]
        [InlineData("123", null, null)]
        public async Task ChangePassword_WithInvalidPasswordsAndConfirmation_ReturnsBadRequest(string currentPass, string newPass, string newPassConfirm)
        {
            //arrange
            var changePasswordRequest = new UserChangePasswordRequest
            {
                CurrentPassword = currentPass,
                NewPassword = newPass,
                NewPasswordConfirm = newPassConfirm
            };
            
            //act
            var httpResponsePatch = await AuthenticatedHttpClient.PatchAsync("/api/Users/Me/Password", GetStringContent(changePasswordRequest));

            Assert.Equal(HttpStatusCode.BadRequest, httpResponsePatch.StatusCode);
        }
        
        [Fact]
        public async Task ChangePassword_WithValidPasswordsAndConfirmation_ReturnsAccepted()
        {
            //arrange
            var user = UserFaker.GetUserCreateRequest();
            
            var userCreated = await CreateUserAsync(user);

            var changePasswordRequest = new UserChangePasswordRequest
            {
                CurrentPassword = user.Password,
                NewPassword = "Ab647477382",
                NewPasswordConfirm = "Ab647477382"
            };

            var httpClient = IntegrationTestsFixture.GetNewHttpClient();

            var tokenResult = AuthUser(userCreated.Email, user.Password);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Token);
            
            //act
            var httpResponsePatch = await httpClient.PatchAsync("/api/Users/Me/Password", GetStringContent(changePasswordRequest));

            //assert
            Assert.Equal(HttpStatusCode.Accepted, httpResponsePatch.StatusCode);
        }
        
        [Fact]
        public async Task ChangePassword_WithInValidCurrentPassword_ReturnsBadRequest()
        {
            //arrange
            var user = UserFaker.GetUserCreateRequest();
            
            var userCreated = await CreateUserAsync(user);

            var changePasswordRequest = new UserChangePasswordRequest
            {
                CurrentPassword = "invalid-pass",
                NewPassword = "Ab647477382",
                NewPasswordConfirm = "Ab647477382"
            };

            var httpClient = IntegrationTestsFixture.GetNewHttpClient();

            var tokenResult = AuthUser(userCreated.Email, user.Password);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Token);
            
            //act
            var httpResponsePatch = await httpClient.PatchAsync("/api/Users/Me/Password", GetStringContent(changePasswordRequest));

            //assert
            Assert.Equal(HttpStatusCode.BadRequest, httpResponsePatch.StatusCode);
        }
        
        private async Task<UserCreateResponse> CreateUserAsync(UserCreateRequest userCreateRequest = null)
        {
            userCreateRequest ??= UserFaker.GetUserCreateRequest();

            var httpResponsePost = await AuthenticatedHttpClient.PostAsync("/api/Users", GetStringContent(userCreateRequest));

            Assert.Equal(HttpStatusCode.Created, httpResponsePost.StatusCode);

            return await GetResultContentAsync<UserCreateResponse>(httpResponsePost);
        }
    }
}
