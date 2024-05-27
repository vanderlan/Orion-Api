using Company.Orion.Application.Core.Commands.UserCreate;
using System.Net;
using System.Threading.Tasks;
using Company.Orion.Test.Integration.Setup;
using Company.Orion.Test.Shared.Faker;
using Company.Orion.Application.Core.Commands.LoginWithCredentials;
using Company.Orion.Application.Core.Commands.LoginWithRefreshToken;
using Xunit;

namespace Company.Orion.Test.Api.V1
{
    public class AuthApiTest : IntegrationTestsBootstrapper
    {
        public AuthApiTest(IntegrationTestsFixture fixture) : base(fixture)
        {
            LoginWithDefaultUser();
        }

        [Theory]
        [InlineData("invalid@email.com", "1233")]
        [InlineData(null, "")]
        [InlineData(null, "passs-invalid")]
        public async Task AuthUser_WithCredentialsInvalid_ReturnsUnauthorized(string email, string password)
        {
            //arrange
            var request = new LoginWithCredentialsRequest
            {
                Email = email,
                Password = password
            };

            //act
            var httpResponse = await HttpClient.PostAsync("/api/Auth/Login", GetStringContent(request));

            //assert
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
        }
        
        [Theory]
        [InlineData("invalid-refresh-token", "1233")]
        [InlineData(null, "")]
        [InlineData(null, "invalid-expired-token")]
        public async Task AuthUser_WithRefreshTokenInvalid_ReturnsUnauthorized(string refreshToken, string token)
        {
            //arrange
            var request = new LoginWithRefreshTokenRequest
            {
                RefreshToken = refreshToken,
                Token = token
            };

            //act
            var httpResponse = await HttpClient.PostAsync("/api/Auth/RefreshToken", GetStringContent(request));

            //assert
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
        }

        [Fact]
        public async Task AuthUser_WithValidRefreshTokenAndExpiredToken_ReturnsNewToken()
        {
            //arrange
            var user = UserFaker.GetUserCreateRequest();
            
            var userCreated = await CreateUserAsync(user);

            var httpClient = IntegrationTestsFixture.GetNewHttpClient();

            var tokenResult = AuthUser(userCreated.Email, user.Password);

            var refreshTokenRequest = new LoginWithRefreshTokenRequest
            {
                RefreshToken = tokenResult.RefreshToken,
                Token = tokenResult.Token
            };
            
            //act
            var httpResponseRefreshToken = await httpClient.PostAsync("/api/Auth/RefreshToken", GetStringContent(refreshTokenRequest));

            var refreshTokenResponse = await GetResultContentAsync<LoginWithRefreshTokenRequest>(httpResponseRefreshToken);
            
            //assert
            Assert.Equal(HttpStatusCode.OK, httpResponseRefreshToken.StatusCode);
            Assert.NotNull(refreshTokenResponse.Token);
            Assert.NotNull(refreshTokenResponse.RefreshToken);
        }
        
        [Fact]
        public async Task AuthUser_WithInvalidRefreshTokenAndValidExpiredToken_ReturnsUnauthorized()
        {
            //arrange
            var user = UserFaker.GetUserCreateRequest();
            
            var userCreated = await CreateUserAsync(user);

            var httpClient = IntegrationTestsFixture.GetNewHttpClient();

            var tokenResult = AuthUser(userCreated.Email, user.Password);

            var refreshTokenRequest = new LoginWithRefreshTokenRequest
            {
                RefreshToken = "invalid-refresh-token",
                Token = tokenResult.Token
            };
            
            //act
            var httpResponseRefreshToken = await httpClient.PostAsync("/api/Auth/RefreshToken", GetStringContent(refreshTokenRequest));
            
            //assert
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponseRefreshToken.StatusCode);
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
