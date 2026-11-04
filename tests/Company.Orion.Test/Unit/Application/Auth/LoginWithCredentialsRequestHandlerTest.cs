using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithCredentials;
using Company.Orion.Crosscutting.Resources;
using Company.Orion.Domain.Core.Authentication;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Exceptions;
using Company.Orion.Domain.Core.Repositories;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using Company.Orion.Domain.Core.Security;
using Company.Orion.Test.Shared.Faker;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;
using static Company.Orion.Crosscutting.Resources.Messages.MessagesKeys;

namespace Company.Orion.Test.Unit.Application.Auth;

public class LoginWithCredentialsRequestHandlerTest
{
    private const string Password = "123";

    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly LoginWithCredentialsRequestHandler _handler;

    private readonly User _user = UserFaker.Get();
    private readonly AccessToken _accessToken = new("jwt-token", DateTime.UtcNow.AddMinutes(15));

    public LoginWithCredentialsRequestHandlerTest()
    {
        _unitOfWorkMock.Setup(unitOfWork => unitOfWork.RefreshTokenRepository).Returns(_refreshTokenRepositoryMock.Object);
        _unitOfWorkMock.Setup(unitOfWork => unitOfWork.UserRepository).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(unitOfWork => unitOfWork.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns((Func<Task> operation, CancellationToken _) => operation());

        _refreshTokenRepositoryMock.Setup(repository => repository.SearchByAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _refreshTokenRepositoryMock.Setup(repository => repository.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns((RefreshToken refreshToken, CancellationToken _) => Task.FromResult(refreshToken));

        _tokenServiceMock.Setup(tokenService => tokenService.CreateAccessToken(_user)).Returns(_accessToken);

        var localizerMock = new Mock<IStringLocalizer<OrionResources>>();
        localizerMock.Setup(localizer => localizer[It.IsAny<string>()])
            .Returns((string key) => new LocalizedString(key, key));

        _handler = new LoginWithCredentialsRequestHandler(_unitOfWorkMock.Object, _passwordHasherMock.Object, _tokenServiceMock.Object, localizerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsTheUserWithAccessAndRefreshTokens()
    {
        //arrange
        SetupUserFound(PasswordVerificationResult.Success);

        RefreshToken addedRefreshToken = null;

        _refreshTokenRepositoryMock.Setup(repository => repository.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Callback((RefreshToken refreshToken, CancellationToken _) => addedRefreshToken = refreshToken)
            .Returns((RefreshToken refreshToken, CancellationToken _) => Task.FromResult(refreshToken));

        //act
        var response = await _handler.Handle(CreateValidRequest(), CancellationToken.None);

        //assert
        Assert.Equal(_user.Email, response.Email);
        Assert.Equal(_user.Name, response.Name);
        Assert.Equal(_user.PublicId, response.PublicId);
        Assert.Equal(_user.Profile, response.Profile);
        Assert.Equal(_accessToken.Token, response.Token);
        Assert.Equal(_accessToken.ExpiresAt, response.Expiration);

        Assert.NotNull(addedRefreshToken);
        Assert.Equal(_user.Email, addedRefreshToken.Email);
        Assert.Equal(addedRefreshToken.Refreshtoken, response.RefreshToken);
        Assert.True(addedRefreshToken.Refreshtoken.Length >= 64);

        _refreshTokenRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidCredentialsAndExistingRefreshTokens_RevokesThemAndIssuesANewOne()
    {
        //arrange
        SetupUserFound(PasswordVerificationResult.Success);

        var existingRefreshToken = RefreshTokenFaker.Get(_user.Email);

        _refreshTokenRepositoryMock.Setup(repository => repository.SearchByAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([existingRefreshToken]);

        //act
        var response = await _handler.Handle(CreateValidRequest(), CancellationToken.None);

        //assert
        Assert.NotEqual(existingRefreshToken.Refreshtoken, response.RefreshToken);
        _refreshTokenRepositoryMock.Verify(repository => repository.DeleteAsync(existingRefreshToken.PublicId, It.IsAny<CancellationToken>()), Times.Once);
        _refreshTokenRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_VerifiesThePasswordAgainstTheStoredHash()
    {
        //arrange
        SetupUserFound(PasswordVerificationResult.Success);

        var request = CreateValidRequest();

        //act
        await _handler.Handle(request, CancellationToken.None);

        //assert
        _userRepositoryMock.Verify(repository => repository.FindByEmailAsync(request.Email, It.IsAny<CancellationToken>()), Times.Once);
        _passwordHasherMock.Verify(hasher => hasher.Verify(_user.Password, request.Password), Times.Once);
        _passwordHasherMock.Verify(hasher => hasher.Hash(It.IsAny<string>()), Times.Never);
        _userRepositoryMock.Verify(repository => repository.Update(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithValidCredentialsAndOutdatedHash_RehashesThePasswordTransparently()
    {
        //arrange
        SetupUserFound(PasswordVerificationResult.SuccessRehashNeeded);

        _passwordHasherMock.Setup(hasher => hasher.Hash(Password)).Returns("new-strong-hash");

        //act
        var response = await _handler.Handle(CreateValidRequest(), CancellationToken.None);

        //assert
        Assert.Equal(_accessToken.Token, response.Token);
        Assert.Equal("new-strong-hash", _user.Password);
        _userRepositoryMock.Verify(repository => repository.Update(_user), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_WithInvalidCredentials_ThrowsUnauthorizedUserException(bool userExists)
    {
        //arrange
        if (userExists)
            SetupUserFound(PasswordVerificationResult.Failed);
        else
            _userRepositoryMock.Setup(repository => repository.FindByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((User)null);

        //act
        var exception = await Assert.ThrowsAsync<UnauthorizedUserException>(() => _handler.Handle(CreateValidRequest(), CancellationToken.None));

        //assert
        Assert.Equal(UserMessages.InvalidCredentials, exception.Message);
        Assert.Equal(ExceptionsTitles.AuthenticationError, exception.Title);
        _tokenServiceMock.Verify(tokenService => tokenService.CreateAccessToken(It.IsAny<User>()), Times.Never);
        _refreshTokenRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsUniqueUrlSafeTokens()
    {
        //arrange & act
        var tokens = new HashSet<string>();

        for (var i = 0; i < 100; i++)
            tokens.Add(LoginWithCredentialsRequestHandler.GenerateRefreshToken());

        //assert
        Assert.Equal(100, tokens.Count);
        Assert.All(tokens, token => Assert.Equal(-1, token.IndexOfAny(['+', '/', '='])));
    }

    private LoginWithCredentialsRequest CreateValidRequest()
    {
        return new LoginWithCredentialsRequest
        {
            Email = _user.Email,
            Password = Password
        };
    }

    private void SetupUserFound(PasswordVerificationResult verificationResult)
    {
        _userRepositoryMock.Setup(repository => repository.FindByEmailAsync(_user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(_user);
        _passwordHasherMock.Setup(hasher => hasher.Verify(_user.Password, Password)).Returns(verificationResult);
    }
}
