using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Company.Orion.Api.Models;
using Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithCredentials;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using Company.Orion.Test.Integration.Setup;
using Company.Orion.Test.Shared.Faker;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithRefreshToken;
using Company.Orion.Application.Core.UseCases.Users.Commands.Create;

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
            var httpResponse = await HttpClient.PostAsync("/Auth/Login", GetStringContent(request));

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
            var httpResponse = await HttpClient.PostAsync("/Auth/RefreshToken", GetStringContent(request));

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
            var httpResponseRefreshToken = await httpClient.PostAsync("/Auth/RefreshToken", GetStringContent(refreshTokenRequest));

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
            var httpResponseRefreshToken = await httpClient.PostAsync("/Auth/RefreshToken", GetStringContent(refreshTokenRequest));
            
            //assert
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponseRefreshToken.StatusCode);
        }
        
        [Fact]
        public async Task AuthUser_WithValidRefreshToken_RotatesTheRefreshToken()
        {
            //arrange
            var user = UserFaker.GetUserCreateRequest();

            var userCreated = await CreateUserAsync(user);

            var httpClient = IntegrationTestsFixture.GetNewHttpClient();

            var tokenResult = AuthUser(userCreated.Email, user.Password);

            //act
            var httpResponseRefreshToken = await httpClient.PostAsync("/Auth/RefreshToken", GetStringContent(
                new LoginWithRefreshTokenRequest
                {
                    RefreshToken = tokenResult.RefreshToken,
                    Token = tokenResult.Token
                }));

            var refreshTokenResponse = await GetResultContentAsync<UserApiTokenModel>(httpResponseRefreshToken);

            //assert: a brand new refresh token must be issued, the old one must not be served again
            Assert.Equal(HttpStatusCode.OK, httpResponseRefreshToken.StatusCode);
            Assert.NotNull(refreshTokenResponse.RefreshToken);
            Assert.NotEqual(tokenResult.RefreshToken, refreshTokenResponse.RefreshToken);
        }

        [Fact]
        public async Task AuthUser_WithAlreadyUsedRefreshToken_ReturnsUnauthorized()
        {
            //arrange
            var user = UserFaker.GetUserCreateRequest();

            var userCreated = await CreateUserAsync(user);

            var httpClient = IntegrationTestsFixture.GetNewHttpClient();

            var tokenResult = AuthUser(userCreated.Email, user.Password);

            var firstRefreshResponse = await httpClient.PostAsync("/Auth/RefreshToken", GetStringContent(
                new LoginWithRefreshTokenRequest
                {
                    RefreshToken = tokenResult.RefreshToken,
                    Token = tokenResult.Token
                }));

            Assert.Equal(HttpStatusCode.OK, firstRefreshResponse.StatusCode);

            //act: tries to use the already rotated (revoked) refresh token again
            var secondRefreshResponse = await httpClient.PostAsync("/Auth/RefreshToken", GetStringContent(
                new LoginWithRefreshTokenRequest
                {
                    RefreshToken = tokenResult.RefreshToken,
                    Token = tokenResult.Token
                }));

            //assert
            Assert.Equal(HttpStatusCode.Unauthorized, secondRefreshResponse.StatusCode);
        }

        [Fact]
        public async Task AuthUser_WithRotatedRefreshToken_CanRefreshAgainWithTheNewToken()
        {
            //arrange
            var user = UserFaker.GetUserCreateRequest();

            var userCreated = await CreateUserAsync(user);

            var httpClient = IntegrationTestsFixture.GetNewHttpClient();

            var tokenResult = AuthUser(userCreated.Email, user.Password);

            var firstRefreshResponse = await httpClient.PostAsync("/Auth/RefreshToken", GetStringContent(
                new LoginWithRefreshTokenRequest
                {
                    RefreshToken = tokenResult.RefreshToken,
                    Token = tokenResult.Token
                }));

            Assert.Equal(HttpStatusCode.OK, firstRefreshResponse.StatusCode);

            var firstRefreshResult = await GetResultContentAsync<UserApiTokenModel>(firstRefreshResponse);

            //act: the newly issued refresh token must be immediately valid
            var secondRefreshResponse = await httpClient.PostAsync("/Auth/RefreshToken", GetStringContent(
                new LoginWithRefreshTokenRequest
                {
                    RefreshToken = firstRefreshResult.RefreshToken,
                    Token = firstRefreshResult.Token
                }));

            var secondRefreshResult = await GetResultContentAsync<UserApiTokenModel>(secondRefreshResponse);

            //assert
            Assert.Equal(HttpStatusCode.OK, secondRefreshResponse.StatusCode);
            Assert.NotNull(secondRefreshResult.Token);
            Assert.NotNull(secondRefreshResult.RefreshToken);
            Assert.NotEqual(firstRefreshResult.RefreshToken, secondRefreshResult.RefreshToken);
        }

        [Fact]
        public async Task AuthUser_AfterRefreshTokenRotation_OnlyTheNewRefreshTokenIsPersisted()
        {
            //arrange
            var user = UserFaker.GetUserCreateRequest();

            var userCreated = await CreateUserAsync(user);

            var httpClient = IntegrationTestsFixture.GetNewHttpClient();

            var tokenResult = AuthUser(userCreated.Email, user.Password);

            //act
            var httpResponseRefreshToken = await httpClient.PostAsync("/Auth/RefreshToken", GetStringContent(
                new LoginWithRefreshTokenRequest
                {
                    RefreshToken = tokenResult.RefreshToken,
                    Token = tokenResult.Token
                }));

            var refreshTokenResponse = await GetResultContentAsync<UserApiTokenModel>(httpResponseRefreshToken);

            //assert: the old token must have been revoked and only the new one must remain valid
            Assert.Equal(HttpStatusCode.OK, httpResponseRefreshToken.StatusCode);

            using var unitOfWork = ServiceProvider.GetRequiredService<IUnitOfWork>();

            var persistedRefreshTokens = (await unitOfWork.RefreshTokenRepository.SearchByAsync(x => x.Email == userCreated.Email)).ToList();

            var persistedRefreshToken = Assert.Single(persistedRefreshTokens);
            Assert.Equal(refreshTokenResponse.RefreshToken, persistedRefreshToken.Refreshtoken);
        }

        private async Task<UserCreateResponse> CreateUserAsync(UserCreateRequest userCreateRequest = null)
        {
            userCreateRequest ??= UserFaker.GetUserCreateRequest();

            var httpResponsePost = await AuthenticatedHttpClient.PostAsync("/Users", GetStringContent(userCreateRequest));

            Assert.Equal(HttpStatusCode.Created, httpResponsePost.StatusCode);

            return await GetResultContentAsync<UserCreateResponse>(httpResponsePost);
        }
    }
}
