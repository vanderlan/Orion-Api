using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Orion.Api.Models;
using Orion.Domain.Core.Services.Interfaces;
using Xunit;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using MediatR;
using Orion.Test.Api.Controllers.BaseController;
using Orion.Domain.Core.Entities;
using Orion.Api.Controllers.V1;
using Orion.Api.Configuration;
using Orion.Application.Core.Commands.LoginWithCredentials;
using Orion.Application.Core.Commands.LoginWithRefreshToken;
using Orion.Test.Faker;

namespace Orion.Test.Api.Controllers;

public class AuthControllerTest : BaseControllerTest
{
    private AuthController _authController;
    private IConfiguration _configuration;
    private readonly User _validUser = UserFaker.Get();
    private readonly RefreshToken _validRefreshToken;

    public AuthControllerTest()
    {
        _validRefreshToken = RefreshTokenFaker.Get(_validUser.Email);
        SetupServiceMock();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOk()
    {
        //arrange & act
        var result = await _authController.Login(
            new LoginWithCredentialsRequest
            {
                Email = _validUser.Email,
                Password = _validUser.Password
            }
        );

        var contentResult = (OkObjectResult)result;
        var userApiToken = (UserApiTokenModel)contentResult.Value;

        //assert
        Assert.IsType<OkObjectResult>(contentResult);
        Assert.Equal(200, contentResult.StatusCode);

        Assert.IsType<UserApiTokenModel>(contentResult.Value);
        Assert.NotNull(userApiToken.Token);
        Assert.True(userApiToken.Expiration > DateTime.Now);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_RetunsUnauthorized()
    {
        //arrange & act
        var result = await _authController.Login(
            new LoginWithCredentialsRequest
            {
                Email = "invalid-login",
                Password = "invalid-pass"
            }
        );

        var contentResult = (UnauthorizedResult)result;

        //assert
        Assert.IsType<UnauthorizedResult>(contentResult);
        Assert.Equal(401, contentResult.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_WithValidRefreshToken_ReturnsNewToken()
    {
        //arrange & act
        var (token, _) = AuthenticationConfiguration.CreateToken(new LoginWithCredentialsResponse { Email = _validUser.Email, Name = _validUser.Name, PublicId = _validUser.PublicId }, _configuration);

        var result = await _authController.RefreshToken(new LoginWithRefreshTokenRequest { RefreshToken = _validRefreshToken.Refreshtoken, Token = token});

        var contentResult = (OkObjectResult)result;
        var userApiToken = (UserApiTokenModel)contentResult.Value;

        //asert
        Assert.IsType<OkObjectResult>(contentResult);
        Assert.Equal(200, contentResult.StatusCode);

        Assert.IsType<UserApiTokenModel>(contentResult.Value);
        Assert.NotNull(userApiToken.Token);
        Assert.True(userApiToken.Expiration > DateTime.Now);
    }

    [Fact]
    public async Task RefreshToken_WithInvalidRefreshToken_ReturnsUnauthorized()
    {
        //arrange & act
        var result = await _authController.RefreshToken(
            new LoginWithRefreshTokenRequest { RefreshToken = null }
        );

        var contentResult = (UnauthorizedResult)result;

        //assert
        Assert.IsType<UnauthorizedResult>(contentResult);
        Assert.Equal(401, contentResult.StatusCode);
    }

    private void SetupServiceMock()
    {
        var mediatotMock = new Mock<IMediator>();
        var userServiceMock = new Mock<IUserService>();

        userServiceMock.Setup(x => x.LoginAsync(_validUser.Email, _validUser.Password))
            .ReturnsAsync(_validUser);
        
        userServiceMock.Setup(x => x.AddRefreshTokenAsync(It.IsAny<RefreshToken>())).ReturnsAsync(RefreshTokenFaker.Get());
        userServiceMock.Setup(x => x.SignInWithRehreshTokenAsync(_validRefreshToken.Refreshtoken, It.IsAny<string>())).ReturnsAsync(_validUser);

        var inMemorySettings = new Dictionary<string, string> {
            {"JwtOptions:SymmetricSecurityKey", "5cCI6IkpXVCJ9.eyJlbWFpbCI6InZhbmRlcmxhbi5nc0BnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJhZG1p"},
            {"JwtOptions:Issuer", "http://www.myapplication.com"},
            {"JwtOptions:Audience", "http://www.myapplication.com"},
            {"JwtOptions:TokenExpirationMinutes", "15"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _authController = new AuthController(userServiceMock.Object,mediatotMock.Object, _configuration);
    }
}
