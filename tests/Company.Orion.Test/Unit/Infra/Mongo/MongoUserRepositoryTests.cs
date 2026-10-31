using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Extensions;
using Company.Orion.Domain.Core.Filters;
using Company.Orion.Infra.Data.Mongo.Repository.Implementations;
using Company.Orion.Test.Shared.Faker;
using Company.Orion.Test.Unit.Infra.Mongo.Setup;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace Company.Orion.Test.Unit.Infra.Mongo;

public class MongoUserRepositoryTests
{
    private readonly MongoMockContext _mongo = new();

    [Fact]
    public async Task LoginAsync_FiltersByEmailAndPassword()
    {
        //arrange
        var user = UserFaker.GetDefaultSystemUser();
        var capture = MongoMockContext.SetupFind(_mongo.Users, user);
        var repository = new MongoUserRepository(_mongo.CreateDataContext());

        //act
        var found = await repository.LoginAsync(user.Email, "123".ToSha512());

        //assert
        Assert.Same(user, found);
        Assert.Equal(user.Email, capture.RenderedFilter[nameof(User.Email)].AsString);
        Assert.Equal("123".ToSha512(), capture.RenderedFilter[nameof(User.Password)].AsString);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ReturnsNull()
    {
        //arrange
        MongoMockContext.SetupFind(_mongo.Users);
        var repository = new MongoUserRepository(_mongo.CreateDataContext());

        //act
        var found = await repository.LoginAsync("invalid@orion.com", "invalid".ToSha512());

        //assert
        Assert.Null(found);
    }

    [Fact]
    public async Task FindByEmailAsync_FiltersByEmailOnly()
    {
        //arrange
        var user = UserFaker.Get();
        var capture = MongoMockContext.SetupFind(_mongo.Users, user);
        var repository = new MongoUserRepository(_mongo.CreateDataContext());

        //act
        var found = await repository.FindByEmailAsync(user.Email);

        //assert
        Assert.Same(user, found);
        Assert.Equal(user.Email, capture.RenderedFilter[nameof(User.Email)].AsString);
        Assert.Single(capture.FilterFields);
    }

    [Fact]
    public async Task ListPaginateAsync_WithoutQuery_UsesAnEmptyFilterOrderedByCreatedAt()
    {
        //arrange
        var users = new[] { UserFaker.Get(), UserFaker.Get(), UserFaker.Get() };
        var capture = MongoMockContext.SetupFind(_mongo.Users, users);

        _mongo.Users.Setup(collection => collection.CountDocumentsAsync(It.IsAny<FilterDefinition<User>>(), It.IsAny<CountOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(42);

        var repository = new MongoUserRepository(_mongo.CreateDataContext());

        //act
        var result = await repository.ListPaginateAsync(new UserFilter { Page = 1, Quantity = 3 });

        //assert
        Assert.Equal(3, result.Items.Count);
        Assert.Equal(42, result.Count);
        Assert.Empty(capture.FilterFields);
        Assert.Equal((int?)0, capture.Options.Skip);
        Assert.Equal((int?)3, capture.Options.Limit);
        Assert.Equal(1, MongoMockContext.Render(capture.Options.Sort)[nameof(User.CreatedAt)].AsInt32);
    }

    [Theory]
    [InlineData(1, 10, 0)]
    [InlineData(2, 10, 10)]
    [InlineData(5, 7, 28)]
    public async Task ListPaginateAsync_CalculatesTheSkipFromPageAndQuantity(int page, int quantity, int expectedSkip)
    {
        //arrange
        var capture = MongoMockContext.SetupFind(_mongo.Users);

        _mongo.Users.Setup(collection => collection.CountDocumentsAsync(It.IsAny<FilterDefinition<User>>(), It.IsAny<CountOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var repository = new MongoUserRepository(_mongo.CreateDataContext());

        //act
        var result = await repository.ListPaginateAsync(new UserFilter { Page = page, Quantity = quantity });

        //assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.Count);
        Assert.Equal((int?)expectedSkip, capture.Options.Skip);
        Assert.Equal((int?)quantity, capture.Options.Limit);
    }

    [Fact]
    public async Task ListPaginateAsync_WithQuery_FiltersTheNameWithACaseInsensitiveEscapedRegex()
    {
        //arrange
        const string query = "John (Admin)";
        var capture = MongoMockContext.SetupFind(_mongo.Users, UserFaker.Get());

        FilterDefinition<User> countFilter = null;

        _mongo.Users.Setup(collection => collection.CountDocumentsAsync(It.IsAny<FilterDefinition<User>>(), It.IsAny<CountOptions>(), It.IsAny<CancellationToken>()))
            .Callback<FilterDefinition<User>, CountOptions, CancellationToken>((filter, _, _) => countFilter = filter)
            .ReturnsAsync(1);

        var repository = new MongoUserRepository(_mongo.CreateDataContext());

        //act
        var result = await repository.ListPaginateAsync(new UserFilter { Query = query });

        //assert
        Assert.Single(result.Items);
        Assert.Equal(1, result.Count);

        var regex = capture.RenderedFilter[nameof(User.Name)].AsBsonRegularExpression;
        Assert.Equal(Regex.Escape(query), regex.Pattern);
        Assert.Equal("i", regex.Options);

        //the count must use exactly the same filter of the list
        Assert.Equal(capture.RenderedFilter, MongoMockContext.Render(countFilter));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ListPaginateAsync_WithBlankQuery_DoesNotFilterTheName(string query)
    {
        //arrange
        var capture = MongoMockContext.SetupFind(_mongo.Users);

        _mongo.Users.Setup(collection => collection.CountDocumentsAsync(It.IsAny<FilterDefinition<User>>(), It.IsAny<CountOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var repository = new MongoUserRepository(_mongo.CreateDataContext());

        //act
        await repository.ListPaginateAsync(new UserFilter { Query = query });

        //assert
        Assert.Equal(new BsonDocument(), capture.RenderedFilter);
    }
}
