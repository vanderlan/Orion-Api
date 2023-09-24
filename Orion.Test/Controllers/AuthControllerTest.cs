using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Orion.Api.Controllers;
using Orion.Api.Models;
using Orion.Entities.Domain;
using Orion.Domain.Interfaces;
using Orion.Test.Controllers.BaseController;
using Orion.Test.MotherObjects;
using Xunit;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Orion.Test.Controllers
{
    public class AuthControllerTest : BaseControllerTest
    {
        private AuthController _authController;

        public AuthControllerTest()
        {
            SetupServiceMock();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOk()
        {
            var result = await _authController.Login(
                new UserLoginModel 
                { 
                    Email = UserMotherObject.ValidAdminUser().Email, 
                    Password =  UserMotherObject.ValidAdminUser().Password
                }
            );

            var contentResult = (OkObjectResult) result;
            var userApiToken = (UserApiTokenModel) contentResult.Value;

            Assert.IsType<OkObjectResult>(contentResult);
            Assert.Equal(200, contentResult.StatusCode);

            Assert.IsType<UserApiTokenModel>(contentResult.Value);
            Assert.NotNull(userApiToken.Token);
            Assert.True(userApiToken.Expiration > DateTime.Now);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_RetunsUnauthorized()
        {
            var result = await _authController.Login(
                new UserLoginModel
                {
                    Email = "invalid-login",
                    Password = "invalid-pass"
                }
            );

            var contentResult = (UnauthorizedResult)result;

            Assert.IsType<UnauthorizedResult>(contentResult);
            Assert.Equal(401, contentResult.StatusCode);
        }

        [Fact]
        public async Task RefreshToken_WithValidRefreshToken_ReturnsNewToken()
        {
            var result = await _authController.RefreshToken(
                RefreshTokenMotherObject.ValidRefreshTokenModel()
            );

            var contentResult = (OkObjectResult)result;
            var userApiToken = (UserApiTokenModel)contentResult.Value;

            Assert.IsType<OkObjectResult>(contentResult);
            Assert.Equal(200, contentResult.StatusCode);

            Assert.IsType<UserApiTokenModel>(contentResult.Value);
            Assert.NotNull(userApiToken.Token);
            Assert.True(userApiToken.Expiration > DateTime.Now);
        }
        [Fact]
        public async Task RefreshToken_WithInvalidRefreshToken_ReturnsUnauthorized()
        {
            var result = await _authController.RefreshToken(
                new RefreshTokenModel { RefreshToken = null}
            );

            var contentResult = (UnauthorizedResult)result;

            Assert.IsType<UnauthorizedResult>(contentResult);
            Assert.Equal(401, contentResult.StatusCode);
        }

        private void SetupServiceMock()
        {
            var userServiceMock = new Mock<IUserService>();

            userServiceMock.Setup(x => x.LoginAsync(UserMotherObject.ValidAdminUser().Email, UserMotherObject.ValidAdminUser().Password))
                .ReturnsAsync(UserMotherObject.ValidAdminUser());

            userServiceMock.Setup(x => x.AddRefreshTokenAsync(It.IsAny<RefreshToken>())).ReturnsAsync(RefreshTokenMotherObject.ValidRefreshToken());
            userServiceMock.Setup(x => x.GetUserByRefreshTokenAsync(RefreshTokenMotherObject.ValidRefreshToken().Refreshtoken)).ReturnsAsync(UserMotherObject.ValidAdminUser());

            //Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"JwtConfiguration:SymmetricSecurityKey", "5cCI6IkpXVCJ9.eyJlbWFpbCI6InZhbmRlcmxhbi5nc0BnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJhZG1p"},
                {"JwtConfiguration:Issuer", "http://www.myapplication.com"},
                {"JwtConfiguration:Audience", "http://www.myapplication.com"},
                {"JwtConfiguration:TokenExpirationMinutes", "15"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _authController = new AuthController(userServiceMock.Object, Mapper, configuration);
        }
    }
}
