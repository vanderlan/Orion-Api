using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Company.Orion.Api.Configuration;
using Company.Orion.Api.Controllers.V1;
using Company.Orion.Api.Models;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Entities.Enuns;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Company.Orion.Test.Shared.Faker;
using Company.Orion.Test.Unit.Controllers.BaseController;
using Xunit;
using Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithCredentials;
using Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithRefreshToken;

namespace Company.Orion.Test.Unit.Controllers;

public class AuthControllerTest : BaseControllerTest
{
    private AuthController _authController;
    private IConfiguration _configuration;
    private readonly User _validUser = UserFaker.Get();
    private readonly RefreshToken _validRefreshToken;

    public AuthControllerTest()
    {
        _validRefreshToken = RefreshTokenFaker.Get(_validUser.Email);
        SetupMediatorMock();
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
    public async Task RefreshToken_WithValidRefreshToken_ReturnsNewToken()
    {
        //arrange & act
        var (token, _) = AuthenticationConfiguration.CreateToken(new LoginWithCredentialsResponse
        {
            Email = _validUser.Email,
            Name = _validUser.Name,
            PublicId = _validUser.PublicId,
            Profile = UserProfile.Admin
        },_configuration);

        var result = await _authController.RefreshToken(new LoginWithRefreshTokenRequest { RefreshToken = _validRefreshToken.Refreshtoken, Token = token });

        var contentResult = (OkObjectResult)result;
        var userApiToken = (UserApiTokenModel)contentResult.Value;

        //asert
        Assert.IsType<OkObjectResult>(contentResult);
        Assert.Equal(200, contentResult.StatusCode);

        Assert.IsType<UserApiTokenModel>(contentResult.Value);
        Assert.NotNull(userApiToken.Token);
        Assert.True(userApiToken.Expiration > DateTime.Now);
    }

    private void SetupMediatorMock()
    {
        var mediatorMock = new Mock<IMediator>();

        mediatorMock.Setup(x => x.Send(It.Is<LoginWithCredentialsRequest>(x => x.Password == _validUser.Password && x.Email == _validUser.Email),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginWithCredentialsResponse
            {
                Email = _validUser.Email,
                Name = _validUser.Name,
                Profile = _validUser.Profile,
                PublicId = _validUser.PublicId,
                RefreshToken = _validRefreshToken.Refreshtoken
            });

        mediatorMock.Setup(x => x.Send(It.Is<LoginWithRefreshTokenRequest>(x => x.RefreshToken == _validRefreshToken.Refreshtoken),
           It.IsAny<CancellationToken>()))
           .ReturnsAsync(new LoginWithRefreshTokenResponse
           {
               Email = _validUser.Email,
               Name = _validUser.Name,
               Profile = _validUser.Profile,
               PublicId = _validUser.PublicId,
               RefreshToken = _validRefreshToken.Refreshtoken
           });

        var inMemorySettings = new Dictionary<string, string> {
           {"JwtOptions:SymmetricSecurityKey", "5cCI6IkpXVCJ9.eyJlbWFpbCI6InZhbmRlcmxhbi5nc0BnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJhZG1p"},
           {"JwtOptions:Issuer", "http://www.myapplication.com"},
           {"JwtOptions:Audience", "http://www.myapplication.com"},
           {"JwtOptions:TokenExpirationMinutes", "15"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _authController = new AuthController(mediatorMock.Object, _configuration);
    }
}
