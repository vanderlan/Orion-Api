using System;
using System.Collections.Generic;
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

public class MongoDataContextTests
{
    private readonly MongoMockContext _mongo = new();

    [Fact]
    public void Constructor_WithNullClient_ThrowsArgumentNullException()
    {
        //arrange & act & assert
        Assert.Throws<ArgumentNullException>(() => new MongoDataContext(null, _mongo.Options));
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        //arrange & act & assert
        Assert.Throws<ArgumentNullException>(() => new MongoDataContext(_mongo.Client.Object, null));
    }

    [Fact]
    public void Constructor_UsesTheConfiguredDatabase()
    {
        //arrange & act
        var dataContext = _mongo.CreateDataContext();

        //assert
        Assert.Same(_mongo.Client.Object, dataContext.Client);
        Assert.Same(_mongo.Database.Object, dataContext.Database);
        _mongo.Client.Verify(client => client.GetDatabase(MongoMockContext.DatabaseName, It.IsAny<MongoDatabaseSettings>()), Times.Once);
    }

    [Fact]
    public void Collections_UseTheEntityNameAsCollectionName()
    {
        //arrange
        var dataContext = _mongo.CreateDataContext();

        //act & assert
        Assert.Same(_mongo.Users.Object, dataContext.Users);
        Assert.Same(_mongo.RefreshTokens.Object, dataContext.RefreshTokens);
        Assert.Same(_mongo.Users.Object, dataContext.GetCollection<User>());
        Assert.Same(_mongo.RefreshTokens.Object, dataContext.GetCollection<RefreshToken>());
    }

    [Fact]
    public void Insert_DoesNotWriteUntilSaveChanges()
    {
        //arrange
        var dataContext = _mongo.CreateDataContext();

        //act
        dataContext.Insert(UserFaker.Get());

        //assert
        Assert.True(dataContext.HasPendingWrites);
        _mongo.Users.Verify(collection => collection.InsertOneAsync(It.IsAny<User>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void Insert_WithNullEntity_ThrowsArgumentNullException()
    {
        //arrange
        var dataContext = _mongo.CreateDataContext();

        //act & assert
        Assert.Throws<ArgumentNullException>(() => dataContext.Insert<User>(null));
        Assert.False(dataContext.HasPendingWrites);
    }

    [Fact]
    public async Task SaveChangesAsync_WithoutPendingWrites_ReturnsZero()
    {
        //arrange
        var dataContext = _mongo.CreateDataContext();

        //act
        var affected = await dataContext.SaveChangesAsync();

        //assert
        Assert.Equal(0, affected);
        _mongo.Users.VerifyNoOtherCalls();
        _mongo.RefreshTokens.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SaveChangesAsync_WithInsert_FillsTimestampsAndPublicIdAndInsertsTheDocument()
    {
        //arrange
        var user = UserFaker.Get();
        var originalPublicId = user.PublicId;
        var before = DateTime.UtcNow.AddSeconds(-1);

        _mongo.Users.Setup(collection => collection.InsertOneAsync(user, It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var dataContext = _mongo.CreateDataContext();
        dataContext.Insert(user);

        //act
        var affected = await dataContext.SaveChangesAsync();

        //assert
        Assert.Equal(1, affected);
        Assert.False(dataContext.HasPendingWrites);
        Assert.NotEqual(originalPublicId, user.PublicId);
        Assert.True(Guid.TryParse(user.PublicId, out _));
        Assert.True(user.CreatedAt >= before);
        Assert.True(user.LastUpdated >= before);
        Assert.Equal(DateTimeKind.Utc, user.CreatedAt.Kind);
        Assert.Equal(DateTimeKind.Utc, user.LastUpdated.Kind);
        _mongo.Users.Verify(collection => collection.InsertOneAsync(user, It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SaveChangesAsync_WithInsert_TruncatesTimestampsToMilliseconds()
    {
        //arrange
        var user = UserFaker.Get();

        _mongo.Users.Setup(collection => collection.InsertOneAsync(user, It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var dataContext = _mongo.CreateDataContext();
        dataContext.Insert(user);

        //act
        await dataContext.SaveChangesAsync();

        //assert
        Assert.Equal(0, user.CreatedAt.Ticks % TimeSpan.TicksPerMillisecond);
        Assert.Equal(0, user.LastUpdated.Ticks % TimeSpan.TicksPerMillisecond);
    }

    [Fact]
    public async Task SaveChangesAsync_WithReplace_UpdatesLastUpdatedAndReplacesTheDocumentById()
    {
        //arrange
        var user = UserFaker.Get();
        var createdAt = new DateTime(2024, 5, 1, 0, 0, 0, DateTimeKind.Utc);
        user.CreatedAt = createdAt;
        user.LastUpdated = createdAt;
        var publicId = user.PublicId;

        FilterDefinition<User> capturedFilter = null;

        _mongo.Users.Setup(collection => collection.ReplaceOneAsync(It.IsAny<FilterDefinition<User>>(), user, It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>()))
            .Callback<FilterDefinition<User>, User, ReplaceOptions, CancellationToken>((filter, _, _, _) => capturedFilter = filter)
            .ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, null));

        var dataContext = _mongo.CreateDataContext();
        dataContext.Replace(user);

        //act
        var affected = await dataContext.SaveChangesAsync();

        //assert
        Assert.Equal(1, affected);
        Assert.Equal(publicId, user.PublicId);
        Assert.Equal(createdAt, user.CreatedAt);
        Assert.True(user.LastUpdated > createdAt);
        Assert.Equal(publicId, MongoMockContext.Render(capturedFilter)["_id"].AsString);
    }

    [Fact]
    public async Task SaveChangesAsync_WithDelete_DeletesTheDocumentByIdWithoutTouchingTheEntity()
    {
        //arrange
        var refreshToken = RefreshTokenFaker.Get();
        var lastUpdated = new DateTime(2024, 5, 1, 0, 0, 0, DateTimeKind.Utc);
        refreshToken.LastUpdated = lastUpdated;

        FilterDefinition<RefreshToken> capturedFilter = null;

        _mongo.RefreshTokens.Setup(collection => collection.DeleteOneAsync(It.IsAny<FilterDefinition<RefreshToken>>(), It.IsAny<CancellationToken>()))
            .Callback<FilterDefinition<RefreshToken>, CancellationToken>((filter, _) => capturedFilter = filter)
            .ReturnsAsync(new DeleteResult.Acknowledged(1));

        var dataContext = _mongo.CreateDataContext();
        dataContext.Delete(refreshToken);

        //act
        var affected = await dataContext.SaveChangesAsync();

        //assert
        Assert.Equal(1, affected);
        Assert.Equal(lastUpdated, refreshToken.LastUpdated);
        Assert.Equal(refreshToken.PublicId, MongoMockContext.Render(capturedFilter)["_id"].AsString);
    }

    [Fact]
    public async Task SaveChangesAsync_WithUnacknowledgedResults_ReturnsZeroAffectedDocuments()
    {
        //arrange
        _mongo.Users.Setup(collection => collection.ReplaceOneAsync(It.IsAny<FilterDefinition<User>>(), It.IsAny<User>(), It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ReplaceOneResult.Unacknowledged.Instance);

        _mongo.Users.Setup(collection => collection.DeleteOneAsync(It.IsAny<FilterDefinition<User>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(DeleteResult.Unacknowledged.Instance);

        var dataContext = _mongo.CreateDataContext();
        dataContext.Replace(UserFaker.Get());
        dataContext.Delete(UserFaker.Get());

        //act
        var affected = await dataContext.SaveChangesAsync();

        //assert
        Assert.Equal(0, affected);
    }

    [Fact]
    public async Task SaveChangesAsync_WithMultipleWrites_ExecutesAllInOrder()
    {
        //arrange
        var callOrder = new List<string>();

        _mongo.Users.Setup(collection => collection.InsertOneAsync(It.IsAny<User>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("insert-user"))
            .Returns(Task.CompletedTask);

        _mongo.RefreshTokens.Setup(collection => collection.DeleteOneAsync(It.IsAny<FilterDefinition<RefreshToken>>(), It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("delete-refresh-token"))
            .ReturnsAsync(new DeleteResult.Acknowledged(1));

        _mongo.Users.Setup(collection => collection.ReplaceOneAsync(It.IsAny<FilterDefinition<User>>(), It.IsAny<User>(), It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("replace-user"))
            .ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, null));

        var dataContext = _mongo.CreateDataContext();
        dataContext.Insert(UserFaker.Get());
        dataContext.Delete(RefreshTokenFaker.Get());
        dataContext.Replace(UserFaker.Get());

        //act
        var affected = await dataContext.SaveChangesAsync();

        //assert
        Assert.Equal(3, affected);
        Assert.Equal(new[] { "insert-user", "delete-refresh-token", "replace-user" }, callOrder);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenTheWriteFails_KeepsThePendingWritesAndRethrows()
    {
        //arrange
        _mongo.Users.Setup(collection => collection.InsertOneAsync(It.IsAny<User>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException("MongoDb is unavailable"));

        var dataContext = _mongo.CreateDataContext();
        dataContext.Insert(UserFaker.Get());

        //act
        var exception = await Assert.ThrowsAsync<TimeoutException>(() => dataContext.SaveChangesAsync());

        //assert
        Assert.Equal("MongoDb is unavailable", exception.Message);
        Assert.True(dataContext.HasPendingWrites);
    }

    [Fact]
    public async Task DiscardChanges_RemovesThePendingWrites()
    {
        //arrange
        var dataContext = _mongo.CreateDataContext();
        dataContext.Insert(UserFaker.Get());
        dataContext.Replace(UserFaker.Get());

        //act
        dataContext.DiscardChanges();
        var affected = await dataContext.SaveChangesAsync();

        //assert
        Assert.False(dataContext.HasPendingWrites);
        Assert.Equal(0, affected);
        _mongo.Users.VerifyNoOtherCalls();
    }
}
