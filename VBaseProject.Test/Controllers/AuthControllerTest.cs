using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using VBaseProject.Api.Controllers;
using VBaseProject.Api.Models;
using VBaseProject.Entities.Domain;
using VBaseProject.Service.Interfaces;
using VBaseProject.Test.Controllers.BaseController;
using VBaseProject.Test.MotherObjects;
using Xunit;

namespace VBaseProject.Test.Controllers
{
    public class AuthControllerTest : BaseControllerTest
    {
        private AuthController authController;

        public AuthControllerTest()
        {
            SetupServiceMock();
        }

        [Fact]
        public async Task LoginValidTest()
        {
            var result = await authController.Login(
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
        public async Task LoginInvalidTest()
        {
            var result = await authController.Login(
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
        public async Task RefreshTokenValidTest()
        {
            var result = await authController.RefreshToken(
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
        public async Task RefreshTokenInValidTest()
        {
            var result = await authController.RefreshToken(
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

            userServiceMock.Setup(x => x.AddRefreshToken(It.IsAny<RefreshToken>())).ReturnsAsync(RefreshTokenMotherObject.ValidRefreshToken());
            userServiceMock.Setup(x => x.GetUserByRefreshToken(RefreshTokenMotherObject.ValidRefreshToken().Refreshtoken)).ReturnsAsync(UserMotherObject.ValidAdminUser());

            authController = new AuthController(userServiceMock.Object, _mapper);
        }
    }
}
