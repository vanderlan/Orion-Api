using System;
using System.Threading;
using System.Threading.Tasks;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Infra.Data.Mongo.Repository.Implementations;
using Company.Orion.Infra.Data.Mongo.UnitOfWork;
using Company.Orion.Test.Shared.Faker;
using Company.Orion.Test.Unit.Infra.Mongo.Setup;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace Company.Orion.Test.Unit.Infra.Mongo;

public class MongoUnitOfWorkTests
{
    private readonly MongoMockContext _mongo = new();
    private readonly Mock<ILogger<MongoUnitOfWork>> _loggerMock = new();

    private MongoUnitOfWork CreateUnitOfWork()
    {
        return new MongoUnitOfWork(_mongo.Client.Object, _mongo.Options, _loggerMock.Object);
    }

    [Fact]
    public void Repositories_AreCreatedOnceAndShareTheSameContext()
    {
        //arrange
        using var unitOfWork = CreateUnitOfWork();

        //act
        var userRepository = unitOfWork.UserRepository;
        var refreshTokenRepository = unitOfWork.RefreshTokenRepository;

        //assert
        Assert.IsType<MongoUserRepository>(userRepository);
        Assert.IsType<MongoRefreshTokenRepository>(refreshTokenRepository);
        Assert.Same(userRepository, unitOfWork.UserRepository);
        Assert.Same(refreshTokenRepository, unitOfWork.RefreshTokenRepository);
        _mongo.Client.Verify(client => client.GetDatabase(MongoMockContext.DatabaseName, It.IsAny<MongoDatabaseSettings>()), Times.Once);
    }

    [Fact]
    public async Task CommitAsync_PersistsThePendingWritesOfAllRepositories()
    {
        //arrange
        _mongo.Users.Setup(collection => collection.InsertOneAsync(It.IsAny<User>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mongo.RefreshTokens.Setup(collection => collection.InsertOneAsync(It.IsAny<RefreshToken>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        using var unitOfWork = CreateUnitOfWork();

        var user = await unitOfWork.UserRepository.AddAsync(UserFaker.Get());
        var refreshToken = await unitOfWork.RefreshTokenRepository.AddAsync(RefreshTokenFaker.Get(user.Email));

        //act
        await unitOfWork.CommitAsync();

        //assert
        _mongo.Users.Verify(collection => collection.InsertOneAsync(user, It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        _mongo.RefreshTokens.Verify(collection => collection.InsertOneAsync(refreshToken, It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(user.PublicId);
        Assert.NotNull(refreshToken.PublicId);
    }

    [Fact]
    public async Task CommitAsync_WithoutChanges_DoesNotTouchTheDatabase()
    {
        //arrange
        using var unitOfWork = CreateUnitOfWork();

        //act
        await unitOfWork.CommitAsync();

        //assert
        _mongo.Users.VerifyNoOtherCalls();
        _mongo.RefreshTokens.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CommitAsync_WhenTheWriteFails_DiscardsTheChangesAndRethrows()
    {
        //arrange
        _mongo.Users.Setup(collection => collection.InsertOneAsync(It.IsAny<User>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException("MongoDb is unavailable"));

        using var unitOfWork = CreateUnitOfWork();
        await unitOfWork.UserRepository.AddAsync(UserFaker.Get());

        //act
        var exception = await Assert.ThrowsAsync<TimeoutException>(() => unitOfWork.CommitAsync());

        //a new commit must not retry the discarded write
        await unitOfWork.CommitAsync();

        //assert
        Assert.Equal("MongoDb is unavailable", exception.Message);
        _mongo.Users.Verify(collection => collection.InsertOneAsync(It.IsAny<User>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        VerifyLog(LogLevel.Error, Times.Once());
        VerifyLog(LogLevel.Warning, Times.Once());
    }

    [Fact]
    public async Task DiscardChanges_RemovesThePendingWrites()
    {
        //arrange
        using var unitOfWork = CreateUnitOfWork();
        await unitOfWork.UserRepository.AddAsync(UserFaker.Get());

        //act
        unitOfWork.DiscardChanges();
        await unitOfWork.CommitAsync();

        //assert
        _mongo.Users.Verify(collection => collection.InsertOneAsync(It.IsAny<User>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        //arrange
        var unitOfWork = CreateUnitOfWork();

        //act
        unitOfWork.Dispose();
        var exception = Record.Exception(() => unitOfWork.Dispose());

        //assert
        Assert.Null(exception);
    }

    private void VerifyLog(LogLevel logLevel, Times times)
    {
        _loggerMock.Verify(logger => logger.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, type) => true),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            times);
    }
}
