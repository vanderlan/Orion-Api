using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithRefreshToken;
using Company.Orion.Crosscutting.Resources;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Exceptions;
using Company.Orion.Domain.Core.Repositories;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using Company.Orion.Test.Shared.Faker;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;
using static Company.Orion.Crosscutting.Resources.Messages.MessagesKeys;

namespace Company.Orion.Test.Unit.Application.Auth;

public class LoginWithRefreshTokenRequestHandlerTest
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly LoginWithRefreshTokenRequestHandler _handler;

    private readonly User _user = UserFaker.Get();
    private readonly RefreshToken _currentRefreshToken;

    public LoginWithRefreshTokenRequestHandlerTest()
    {
        _currentRefreshToken = RefreshTokenFaker.Get(_user.Email);

        _unitOfWorkMock.Setup(unitOfWork => unitOfWork.RefreshTokenRepository).Returns(_refreshTokenRepositoryMock.Object);
        _unitOfWorkMock.Setup(unitOfWork => unitOfWork.UserRepository).Returns(_userRepositoryMock.Object);

        //By default the transaction just executes the operation, the transactional behavior
        //itself is covered by the unit of work and integration tests.
        _unitOfWorkMock.Setup(unitOfWork => unitOfWork.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
            .Returns((Func<Task> operation) => operation());

        _unitOfWorkMock.Setup(unitOfWork => unitOfWork.CommitAsync()).Returns(Task.CompletedTask);

        var localizerMock = new Mock<IStringLocalizer<OrionResources>>();
        localizerMock.Setup(localizer => localizer[It.IsAny<string>()])
            .Returns((string key) => new LocalizedString(key, key));

        _handler = new LoginWithRefreshTokenRequestHandler(_unitOfWorkMock.Object, localizerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidTokenAndRefreshToken_RotatesTheRefreshToken()
    {
        //arrange
        SetupRefreshTokenFound();
        SetupUserFound();

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

        //the old token must be revoked and a brand new one must be returned
        Assert.NotNull(response.RefreshToken);
        Assert.NotEqual(_currentRefreshToken.Refreshtoken, response.RefreshToken);
        Assert.NotNull(addedRefreshToken);
        Assert.Equal(addedRefreshToken.Refreshtoken, response.RefreshToken);
        Assert.Equal(_user.Email, addedRefreshToken.Email);

        _refreshTokenRepositoryMock.Verify(repository => repository.DeleteAsync(_currentRefreshToken.PublicId), Times.Once);
        _refreshTokenRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidRotation_ReturnsTheNewTokenEvenWhenTheOldOneIsStillReturnedByTheDatabase()
    {
        //arrange
        //simulates the original consistency problem: the old token is always returned by the
        //lookups because the deletion was not persisted yet. The rotation must not rely on a
        //new lookup and must always return the newly created token.
        _refreshTokenRepositoryMock.Setup(repository => repository.SearchByAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
            .ReturnsAsync([_currentRefreshToken]);

        SetupUserFound();
        SetupAddReturningTheGivenToken();

        //act
        var response = await _handler.Handle(CreateValidRequest(), CancellationToken.None);

        //assert
        Assert.NotEqual(_currentRefreshToken.Refreshtoken, response.RefreshToken);
    }

    [Fact]
    public async Task Handle_WithValidRotation_RevokesTheOldTokenBeforeAddingTheNewOneAndCommitsInsideTheTransaction()
    {
        //arrange
        SetupRefreshTokenFound();
        SetupUserFound();

        var executionOrder = new List<string>();
        var insideTransaction = false;

        _unitOfWorkMock.Setup(unitOfWork => unitOfWork.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
            .Returns(async (Func<Task> operation) =>
            {
                insideTransaction = true;
                await operation();
                insideTransaction = false;
            });

        _refreshTokenRepositoryMock.Setup(repository => repository.DeleteAsync(It.IsAny<string>()))
            .Callback(() => executionOrder.Add(insideTransaction ? "delete" : "delete-outside-transaction"))
            .Returns(Task.CompletedTask);

        _refreshTokenRepositoryMock.Setup(repository => repository.AddAsync(It.IsAny<RefreshToken>()))
            .Callback(() => executionOrder.Add(insideTransaction ? "add" : "add-outside-transaction"))
            .Returns((RefreshToken refreshToken) => Task.FromResult(refreshToken));

        _unitOfWorkMock.Setup(unitOfWork => unitOfWork.CommitAsync())
            .Callback(() => executionOrder.Add(insideTransaction ? "commit" : "commit-outside-transaction"))
            .Returns(Task.CompletedTask);

        //act
        await _handler.Handle(CreateValidRequest(), CancellationToken.None);

        //assert
        Assert.Equal(new[] { "delete", "add", "commit" }, executionOrder);
    }

    [Fact]
    public async Task Handle_WithValidTokenAndRefreshToken_SearchesTheRefreshTokenByTokenAndEmailOfTheJwtToken()
    {
        //arrange
        Expression<Func<RefreshToken, bool>> capturedPredicate = null;

        _refreshTokenRepositoryMock.Setup(repository => repository.SearchByAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
            .Callback((Expression<Func<RefreshToken, bool>> predicate) => capturedPredicate = predicate)
            .ReturnsAsync([_currentRefreshToken]);

        SetupUserFound();
        SetupAddReturningTheGivenToken();

        //act
        await _handler.Handle(CreateValidRequest(), CancellationToken.None);

        //assert
        Assert.NotNull(capturedPredicate);

        var predicate = capturedPredicate.Compile();

        var sameTokenOfAnotherUser = new RefreshToken
        {
            Email = "another@email.com",
            Refreshtoken = _currentRefreshToken.Refreshtoken
        };

        Assert.True(predicate(_currentRefreshToken));

        //a different token of the same user must not match
        Assert.False(predicate(RefreshTokenFaker.Get(_user.Email)));

        //the same token of another user must not match
        Assert.False(predicate(sameTokenOfAnotherUser));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Handle_WithEmptyRefreshToken_ThrowsUnauthorizedUserException(string refreshToken)
    {
        //arrange
        var request = new LoginWithRefreshTokenRequest
        {
            Token = CreateJwtToken(_user.Email),
            RefreshToken = refreshToken
        };

        //act
        var exception = await Assert.ThrowsAsync<UnauthorizedUserException>(() => _handler.Handle(request, CancellationToken.None));

        //assert
        Assert.Equal(UserMessages.InvalidRefreshToken, exception.Message);
        _refreshTokenRepositoryMock.Verify(repository => repository.SearchByAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()), Times.Never);
        VerifyRotationNeverHappened();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid-jwt-token")]
    public async Task Handle_WithInvalidJwtToken_ThrowsUnauthorizedUserException(string token)
    {
        //arrange
        var request = new LoginWithRefreshTokenRequest
        {
            Token = token,
            RefreshToken = _currentRefreshToken.Refreshtoken
        };

        //act
        var exception = await Assert.ThrowsAsync<UnauthorizedUserException>(() => _handler.Handle(request, CancellationToken.None));

        //assert
        Assert.Equal(UserMessages.InvalidRefreshToken, exception.Message);
        VerifyRotationNeverHappened();
    }

    [Fact]
    public async Task Handle_WithJwtTokenWithoutEmailClaim_ThrowsUnauthorizedUserException()
    {
        //arrange
        var tokenWithoutEmail = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(claims:
        [
            new Claim(ClaimTypes.GivenName, _user.Name)
        ]));

        var request = new LoginWithRefreshTokenRequest
        {
            Token = tokenWithoutEmail,
            RefreshToken = _currentRefreshToken.Refreshtoken
        };

        //act & assert
        await Assert.ThrowsAsync<UnauthorizedUserException>(() => _handler.Handle(request, CancellationToken.None));
        VerifyRotationNeverHappened();
    }

    [Fact]
    public async Task Handle_WithRefreshTokenNotFound_ThrowsUnauthorizedUserException()
    {
        //arrange
        _refreshTokenRepositoryMock.Setup(repository => repository.SearchByAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
            .ReturnsAsync([]);

        //act
        var exception = await Assert.ThrowsAsync<UnauthorizedUserException>(() => _handler.Handle(CreateValidRequest(), CancellationToken.None));

        //assert
        Assert.Equal(UserMessages.InvalidRefreshToken, exception.Message);
        VerifyRotationNeverHappened();
    }

    [Fact]
    public async Task Handle_WithUserNotFound_ThrowsUnauthorizedUserException()
    {
        //arrange
        SetupRefreshTokenFound();

        _userRepositoryMock.Setup(repository => repository.SearchByAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync([]);

        //act
        var exception = await Assert.ThrowsAsync<UnauthorizedUserException>(() => _handler.Handle(CreateValidRequest(), CancellationToken.None));

        //assert
        Assert.Equal(UserMessages.InvalidRefreshToken, exception.Message);
        VerifyRotationNeverHappened();
    }

    [Fact]
    public async Task Handle_WhenTheRotationCannotBePersisted_ThrowsRefreshTokenRotationException()
    {
        //arrange
        SetupRefreshTokenFound();
        SetupUserFound();
        SetupAddReturningTheGivenToken();

        var persistenceException = new TimeoutException("The database is unavailable");

        _unitOfWorkMock.Setup(unitOfWork => unitOfWork.CommitAsync()).ThrowsAsync(persistenceException);

        //act
        var exception = await Assert.ThrowsAsync<RefreshTokenRotationException>(() => _handler.Handle(CreateValidRequest(), CancellationToken.None));

        //assert
        Assert.Equal(UserMessages.RefreshTokenRotationFailed, exception.Message);
        Assert.Equal(ExceptionsTitles.AuthenticationError, exception.Title);
        Assert.Same(persistenceException, exception.InnerException);
    }

    [Fact]
    public async Task Handle_WhenTheTransactionFails_ThrowsRefreshTokenRotationException()
    {
        //arrange
        SetupRefreshTokenFound();
        SetupUserFound();

        var transactionException = new InvalidOperationException("The transaction was aborted");

        _unitOfWorkMock.Setup(unitOfWork => unitOfWork.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
            .ThrowsAsync(transactionException);

        //act
        var exception = await Assert.ThrowsAsync<RefreshTokenRotationException>(() => _handler.Handle(CreateValidRequest(), CancellationToken.None));

        //assert
        Assert.Equal(UserMessages.RefreshTokenRotationFailed, exception.Message);
        Assert.Equal(ExceptionsTitles.AuthenticationError, exception.Title);
        Assert.Same(transactionException, exception.InnerException);
    }

    private LoginWithRefreshTokenRequest CreateValidRequest()
    {
        return new LoginWithRefreshTokenRequest
        {
            Token = CreateJwtToken(_user.Email),
            RefreshToken = _currentRefreshToken.Refreshtoken
        };
    }

    private static string CreateJwtToken(string email)
    {
        var token = new JwtSecurityToken(claims:
        [
            new Claim(ClaimTypes.Email, email)
        ]);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private void SetupRefreshTokenFound()
    {
        _refreshTokenRepositoryMock.SetupSequence(repository => repository.SearchByAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
            .ReturnsAsync([_currentRefreshToken])
            .ReturnsAsync([]);
    }

    private void SetupUserFound()
    {
        _userRepositoryMock.Setup(repository => repository.SearchByAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync([_user]);
    }

    private void SetupAddReturningTheGivenToken()
    {
        _refreshTokenRepositoryMock.Setup(repository => repository.AddAsync(It.IsAny<RefreshToken>()))
            .Returns((RefreshToken refreshToken) => Task.FromResult(refreshToken));
    }

    private void VerifyRotationNeverHappened()
    {
        _refreshTokenRepositoryMock.Verify(repository => repository.DeleteAsync(It.IsAny<string>()), Times.Never);
        _refreshTokenRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<RefreshToken>()), Times.Never);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()), Times.Never);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.CommitAsync(), Times.Never);
    }
}
