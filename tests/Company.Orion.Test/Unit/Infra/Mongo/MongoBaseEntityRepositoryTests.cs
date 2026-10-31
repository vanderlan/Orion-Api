using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Infra.Data.Mongo.Repository.Implementations;
using Company.Orion.Test.Shared.Faker;
using Company.Orion.Test.Unit.Infra.Mongo.Setup;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace Company.Orion.Test.Unit.Infra.Mongo;

/// <summary>
/// Exercises the generic repository through its RefreshToken implementation.
/// </summary>
public class MongoBaseEntityRepositoryTests
{
    private readonly MongoMockContext _mongo = new();

    [Fact]
    public async Task AddAsync_ReturnsTheSameEntityAndBuffersTheInsert()
    {
        //arrange
        var dataContext = _mongo.CreateDataContext();
        var repository = new MongoRefreshTokenRepository(dataContext);
        var refreshToken = RefreshTokenFaker.Get();

        //act
        var added = await repository.AddAsync(refreshToken);

        //assert
        Assert.Same(refreshToken, added);
        Assert.True(dataContext.HasPendingWrites);
        _mongo.RefreshTokens.Verify(collection => collection.InsertOneAsync(It.IsAny<RefreshToken>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_FiltersByTheDocumentIdentifier()
    {
        //arrange
        var refreshToken = RefreshTokenFaker.Get();
        var capture = MongoMockContext.SetupFind(_mongo.RefreshTokens, refreshToken);
        var repository = new MongoRefreshTokenRepository(_mongo.CreateDataContext());

        //act
        var found = await repository.GetByIdAsync(refreshToken.PublicId);

        //assert
        Assert.Same(refreshToken, found);
        Assert.Equal(refreshToken.PublicId, capture.RenderedFilter["_id"].AsString);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        //arrange
        MongoMockContext.SetupFind(_mongo.RefreshTokens);
        var repository = new MongoRefreshTokenRepository(_mongo.CreateDataContext());

        //act
        var found = await repository.GetByIdAsync(Guid.NewGuid().ToString());

        //assert
        Assert.Null(found);
    }

    [Fact]
    public async Task SearchByAsync_TranslatesThePredicateAndReturnsAllMatches()
    {
        //arrange
        var email = "user@orion.com";
        var first = RefreshTokenFaker.Get(email);
        var second = RefreshTokenFaker.Get(email);
        var capture = MongoMockContext.SetupFind(_mongo.RefreshTokens, first, second);
        var repository = new MongoRefreshTokenRepository(_mongo.CreateDataContext());

        //act
        var result = (await repository.SearchByAsync(refreshToken => refreshToken.Email == email)).ToList();

        //assert
        Assert.Equal(2, result.Count);
        Assert.Contains(first, result);
        Assert.Contains(second, result);
        Assert.Equal(email, capture.RenderedFilter[nameof(RefreshToken.Email)].AsString);
    }

    [Fact]
    public async Task SearchByAsync_WithStringEqualsAndMultipleConditions_TranslatesAllFields()
    {
        //arrange
        var refreshToken = RefreshTokenFaker.Get();
        var capture = MongoMockContext.SetupFind(_mongo.RefreshTokens, refreshToken);
        var repository = new MongoRefreshTokenRepository(_mongo.CreateDataContext());

        //act (same predicate shape used by LoginWithRefreshTokenRequestHandler)
        var result = await repository.SearchByAsync(current =>
            current.Refreshtoken.Equals(refreshToken.Refreshtoken) && current.Email == refreshToken.Email);

        //assert
        Assert.Single(result);
        var rendered = capture.RenderedFilter.ToJson();
        Assert.Contains(refreshToken.Refreshtoken, rendered);
        Assert.Contains(refreshToken.Email, rendered);
    }

    [Fact]
    public async Task DeleteAsync_WhenTheEntityExists_BuffersTheDelete()
    {
        //arrange
        var refreshToken = RefreshTokenFaker.Get();
        MongoMockContext.SetupFind(_mongo.RefreshTokens, refreshToken);

        _mongo.RefreshTokens.Setup(collection => collection.DeleteOneAsync(It.IsAny<FilterDefinition<RefreshToken>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteResult.Acknowledged(1));

        var dataContext = _mongo.CreateDataContext();
        var repository = new MongoRefreshTokenRepository(dataContext);

        //act
        await repository.DeleteAsync(refreshToken.PublicId);

        //assert
        Assert.True(dataContext.HasPendingWrites);
        _mongo.RefreshTokens.Verify(collection => collection.DeleteOneAsync(It.IsAny<FilterDefinition<RefreshToken>>(), It.IsAny<CancellationToken>()), Times.Never);

        await dataContext.SaveChangesAsync();

        _mongo.RefreshTokens.Verify(collection => collection.DeleteOneAsync(It.IsAny<FilterDefinition<RefreshToken>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenTheEntityDoesNotExist_DoesNothing()
    {
        //arrange
        MongoMockContext.SetupFind(_mongo.RefreshTokens);
        var dataContext = _mongo.CreateDataContext();
        var repository = new MongoRefreshTokenRepository(dataContext);

        //act
        await repository.DeleteAsync(Guid.NewGuid().ToString());

        //assert
        Assert.False(dataContext.HasPendingWrites);
    }

    [Fact]
    public async Task Update_BuffersTheReplace()
    {
        //arrange
        var refreshToken = RefreshTokenFaker.Get();

        _mongo.RefreshTokens.Setup(collection => collection.ReplaceOneAsync(It.IsAny<FilterDefinition<RefreshToken>>(), refreshToken, It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, null));

        var dataContext = _mongo.CreateDataContext();
        var repository = new MongoRefreshTokenRepository(dataContext);

        //act
        repository.Update(refreshToken);

        //assert
        Assert.True(dataContext.HasPendingWrites);

        await dataContext.SaveChangesAsync();

        _mongo.RefreshTokens.Verify(collection => collection.ReplaceOneAsync(It.IsAny<FilterDefinition<RefreshToken>>(), refreshToken, It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
