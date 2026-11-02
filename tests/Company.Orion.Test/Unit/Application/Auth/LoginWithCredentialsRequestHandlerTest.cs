using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithCredentials;
using Company.Orion.Crosscutting.Resources;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Exceptions;
using Company.Orion.Domain.Core.Extensions;
using Company.Orion.Domain.Core.Repositories;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using Company.Orion.Test.Shared.Faker;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;
using static Company.Orion.Crosscutting.Resources.Messages.MessagesKeys;

namespace Company.Orion.Test.Unit.Application.Auth;

public class LoginWithCredentialsRequestHandlerTest
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly LoginWithCredentialsRequestHandler _handler;

    private readonly User _user = UserFaker.Get();

    public LoginWithCredentialsRequestHandlerTest()
    {
        _unitOfWorkMock.Setup(unitOfWork => unitOfWork.RefreshTokenRepository).Returns(_refreshTokenRepositoryMock.Object);
        _unitOfWorkMock.Setup(unitOfWork => unitOfWork.UserRepository).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(unitOfWork => unitOfWork.CommitAsync()).Returns(Task.CompletedTask);

        var localizerMock = new Mock<IStringLocalizer<OrionResources>>();
        localizerMock.Setup(localizer => localizer[It.IsAny<string>()])
            .Returns((string key) => new LocalizedString(key, key));

        _handler = new LoginWithCredentialsRequestHandler(_unitOfWorkMock.Object, localizerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCredentialsAndNoExistingRefreshToken_CreatesAndPersistsANewRefreshToken()
    {
        //arrange
        SetupLoginReturningTheUser();

        _refreshTokenRepositoryMock.Setup(repository => repository.SearchByAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
            .ReturnsAsync([]);

        RefreshToken addedRefreshToken = null;

        _refreshTokenRepositoryMock.Setup(repository => repository.AddAsync(It.IsAny<RefreshToken>()))
            .Callback((RefreshToken refreshToken) => addedRefreshToken = refreshToken)
            .Returns((RefreshToken refreshToken) => Task.FromResult(refreshToken));

        //act
        var response = await _handler.Handle(CreateValidRequest(), CancellationToken.None);

        //assert
        Assert.Equal(_user.Email, response.Email);
        Assert.Equal(_user.Name, response.Name);
        Assert.Equal(_user.PublicId, response.PublicId);
        Assert.Equal(_user.Profile, response.Profile);

        Assert.NotNull(addedRefreshToken);
        Assert.Equal(_user.Email, addedRefreshToken.Email);
        Assert.Equal(addedRefreshToken.Refreshtoken, response.RefreshToken);

        _refreshTokenRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidCredentialsAndAnExistingRefreshToken_ReusesTheExistingOne()
    {
        //arrange
        SetupLoginReturningTheUser();

        var existingRefreshToken = RefreshTokenFaker.Get(_user.Email);

        _refreshTokenRepositoryMock.Setup(repository => repository.SearchByAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
            .ReturnsAsync([existingRefreshToken]);

        //act
        var response = await _handler.Handle(CreateValidRequest(), CancellationToken.None);

        //assert
        Assert.Equal(existingRefreshToken.Refreshtoken, response.RefreshToken);
        _refreshTokenRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<RefreshToken>()), Times.Never);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_SearchesTheUserByEmailAndHashedPassword()
    {
        //arrange
        SetupLoginReturningTheUser();

        _refreshTokenRepositoryMock.Setup(repository => repository.SearchByAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
            .ReturnsAsync([RefreshTokenFaker.Get(_user.Email)]);

        var request = CreateValidRequest();

        //act
        await _handler.Handle(request, CancellationToken.None);

        //assert
        _userRepositoryMock.Verify(repository => repository.LoginAsync(request.Email, request.Password.ToSha512()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidCredentials_ThrowsUnauthorizedUserException()
    {
        //arrange
        _userRepositoryMock.Setup(repository => repository.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((User)null);

        //act
        var exception = await Assert.ThrowsAsync<UnauthorizedUserException>(() => _handler.Handle(CreateValidRequest(), CancellationToken.None));

        //assert
        Assert.Equal(UserMessages.InvalidCredentials, exception.Message);
        Assert.Equal(ExceptionsTitles.AuthenticationError, exception.Title);
        _refreshTokenRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<RefreshToken>()), Times.Never);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.CommitAsync(), Times.Never);
    }

    private LoginWithCredentialsRequest CreateValidRequest()
    {
        return new LoginWithCredentialsRequest
        {
            Email = _user.Email,
            Password = "123"
        };
    }

    private void SetupLoginReturningTheUser()
    {
        _userRepositoryMock.Setup(repository => repository.LoginAsync(_user.Email, "123".ToSha512()))
            .ReturnsAsync(_user);
    }
}
