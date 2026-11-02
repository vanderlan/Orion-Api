using System;
using System.Threading;
using System.Threading.Tasks;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Infra.Data.Mongo.Context;
using Company.Orion.Test.Shared.Faker;
using Company.Orion.Test.Unit.Infra.Mongo.Setup;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace Company.Orion.Test.Unit.Infra.Mongo;

public class MongoPendingWriteTests
{
    private readonly MongoMockContext _mongo = new();

    [Theory]
    [InlineData(MongoWriteOperation.Insert)]
    [InlineData(MongoWriteOperation.Replace)]
    [InlineData(MongoWriteOperation.Delete)]
    public void Constructor_ExposesTheEntityAndTheOperation(MongoWriteOperation operation)
    {
        //arrange
        var user = UserFaker.Get();

        //act
        var pendingWrite = new MongoPendingWrite<User>(user, operation);

        //assert
        Assert.Same(user, pendingWrite.Entity);
        Assert.Equal(operation, pendingWrite.Operation);
    }

    [Fact]
    public async Task ExecuteAsync_WithInsert_InsertsTheEntityOnItsCollection()
    {
        //arrange
        var user = UserFaker.Get();

        _mongo.Users.Setup(collection => collection.InsertOneAsync(user, It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var pendingWrite = new MongoPendingWrite<User>(user, MongoWriteOperation.Insert);

        //act
        var affected = await pendingWrite.ExecuteAsync(_mongo.CreateDataContext(), CancellationToken.None);

        //assert
        Assert.Equal(1, affected);
        _mongo.Users.Verify(collection => collection.InsertOneAsync(user, It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        _mongo.RefreshTokens.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WithReplace_DoesNotUpsert()
    {
        //arrange
        var user = UserFaker.Get();
        ReplaceOptions capturedOptions = null;

        _mongo.Users.Setup(collection => collection.ReplaceOneAsync(It.IsAny<FilterDefinition<User>>(), user, It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>()))
            .Callback<FilterDefinition<User>, User, ReplaceOptions, CancellationToken>((_, _, options, _) => capturedOptions = options)
            .ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 0, null));

        var pendingWrite = new MongoPendingWrite<User>(user, MongoWriteOperation.Replace);

        //act
        var affected = await pendingWrite.ExecuteAsync(_mongo.CreateDataContext(), CancellationToken.None);

        //assert
        Assert.Equal(0, affected);
        Assert.NotNull(capturedOptions);
        Assert.False(capturedOptions.IsUpsert);
    }

    [Fact]
    public async Task ExecuteAsync_WithDelete_ReturnsTheDeletedCount()
    {
        //arrange
        var refreshToken = RefreshTokenFaker.Get();

        _mongo.RefreshTokens.Setup(collection => collection.DeleteOneAsync(It.IsAny<FilterDefinition<RefreshToken>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteResult.Acknowledged(0));

        var pendingWrite = new MongoPendingWrite<RefreshToken>(refreshToken, MongoWriteOperation.Delete);

        //act
        var affected = await pendingWrite.ExecuteAsync(_mongo.CreateDataContext(), CancellationToken.None);

        //assert
        Assert.Equal(0, affected);
    }

    [Fact]
    public async Task ExecuteAsync_WithInsertAndSession_InsertsThroughTheSession()
    {
        //arrange
        var session = new Mock<IClientSessionHandle>();
        var user = UserFaker.Get();

        _mongo.Users.Setup(collection => collection.InsertOneAsync(session.Object, user, It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var dataContext = _mongo.CreateDataContext();
        dataContext.UseSession(session.Object);

        var pendingWrite = new MongoPendingWrite<User>(user, MongoWriteOperation.Insert);

        //act
        var affected = await pendingWrite.ExecuteAsync(dataContext, CancellationToken.None);

        //assert
        Assert.Equal(1, affected);
        _mongo.Users.Verify(collection => collection.InsertOneAsync(session.Object, user, It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        _mongo.Users.Verify(collection => collection.InsertOneAsync(It.IsAny<User>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithReplaceAndSession_ReplacesThroughTheSession()
    {
        //arrange
        var session = new Mock<IClientSessionHandle>();
        var user = UserFaker.Get();

        _mongo.Users.Setup(collection => collection.ReplaceOneAsync(session.Object, It.IsAny<FilterDefinition<User>>(), user, It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, null));

        var dataContext = _mongo.CreateDataContext();
        dataContext.UseSession(session.Object);

        var pendingWrite = new MongoPendingWrite<User>(user, MongoWriteOperation.Replace);

        //act
        var affected = await pendingWrite.ExecuteAsync(dataContext, CancellationToken.None);

        //assert
        Assert.Equal(1, affected);
        _mongo.Users.Verify(collection => collection.ReplaceOneAsync(session.Object, It.IsAny<FilterDefinition<User>>(), user, It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        _mongo.Users.Verify(collection => collection.ReplaceOneAsync(It.IsAny<FilterDefinition<User>>(), It.IsAny<User>(), It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithDeleteAndSession_DeletesThroughTheSession()
    {
        //arrange
        var session = new Mock<IClientSessionHandle>();
        var refreshToken = RefreshTokenFaker.Get();

        _mongo.RefreshTokens.Setup(collection => collection.DeleteOneAsync(session.Object, It.IsAny<FilterDefinition<RefreshToken>>(), It.IsAny<DeleteOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteResult.Acknowledged(1));

        var dataContext = _mongo.CreateDataContext();
        dataContext.UseSession(session.Object);

        var pendingWrite = new MongoPendingWrite<RefreshToken>(refreshToken, MongoWriteOperation.Delete);

        //act
        var affected = await pendingWrite.ExecuteAsync(dataContext, CancellationToken.None);

        //assert
        Assert.Equal(1, affected);
        _mongo.RefreshTokens.Verify(collection => collection.DeleteOneAsync(session.Object, It.IsAny<FilterDefinition<RefreshToken>>(), It.IsAny<DeleteOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        _mongo.RefreshTokens.Verify(collection => collection.DeleteOneAsync(It.IsAny<FilterDefinition<RefreshToken>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithUnknownOperation_ThrowsNotSupportedException()
    {
        //arrange
        var pendingWrite = new MongoPendingWrite<User>(UserFaker.Get(), (MongoWriteOperation)99);

        //act & assert
        await Assert.ThrowsAsync<NotSupportedException>(() => pendingWrite.ExecuteAsync(_mongo.CreateDataContext(), CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_ForwardsTheCancellationToken()
    {
        //arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var user = UserFaker.Get();

        _mongo.Users.Setup(collection => collection.InsertOneAsync(user, It.IsAny<InsertOneOptions>(), cancellationTokenSource.Token))
            .Returns(Task.CompletedTask);

        var pendingWrite = new MongoPendingWrite<User>(user, MongoWriteOperation.Insert);

        //act
        await pendingWrite.ExecuteAsync(_mongo.CreateDataContext(), cancellationTokenSource.Token);

        //assert
        _mongo.Users.Verify(collection => collection.InsertOneAsync(user, It.IsAny<InsertOneOptions>(), cancellationTokenSource.Token), Times.Once);
    }
}
