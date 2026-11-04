using System;
using System.Threading;
using System.Threading.Tasks;
using Company.Orion.Api.Controllers.V1;
using Company.Orion.Api.Models;
using Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithCredentials;
using Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithRefreshToken;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Test.Shared.Faker;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Company.Orion.Test.Unit.Api.Controllers.V1;

public class AuthControllerTest
{
    private const string AccessToken = "access-token";
    private const string RefreshedAccessToken = "refreshed-access-token";

    private readonly AuthController _authController;
    private readonly User _validUser = UserFaker.Get();
    private readonly RefreshToken _validRefreshToken;
    private readonly DateTime _expiration = DateTime.UtcNow.AddMinutes(15);

    public AuthControllerTest()
    {
        _validRefreshToken = RefreshTokenFaker.Get(_validUser.Email);
        _authController = new AuthController(CreateMediatorMock().Object);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithTheTokens()
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
        Assert.Equal(AccessToken, userApiToken.Token);
        Assert.Equal(_validRefreshToken.Refreshtoken, userApiToken.RefreshToken);
        Assert.Equal(_expiration, userApiToken.Expiration);
    }

    [Fact]
    public async Task RefreshToken_WithValidRefreshToken_ReturnsNewToken()
    {
        //arrange & act
        var result = await _authController.RefreshToken(new LoginWithRefreshTokenRequest
        {
            RefreshToken = _validRefreshToken.Refreshtoken,
            Token = AccessToken
        });

        var contentResult = (OkObjectResult)result;
        var userApiToken = (UserApiTokenModel)contentResult.Value;

        //assert
        Assert.IsType<OkObjectResult>(contentResult);
        Assert.Equal(200, contentResult.StatusCode);

        Assert.IsType<UserApiTokenModel>(contentResult.Value);
        Assert.Equal(RefreshedAccessToken, userApiToken.Token);
        Assert.Equal(_expiration, userApiToken.Expiration);
    }

    private Mock<IMediator> CreateMediatorMock()
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
                Token = AccessToken,
                Expiration = _expiration,
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
               Token = RefreshedAccessToken,
               Expiration = _expiration,
               RefreshToken = _validRefreshToken.Refreshtoken
           });

        return mediatorMock;
    }
}
