using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Infra.Data.Mongo.Configuration;
using Company.Orion.Infra.Data.Mongo.Context;
using Company.Orion.Infra.Data.Mongo.Mapping;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Moq;

namespace Company.Orion.Test.Unit.Infra.Mongo.Setup;

/// <summary>
/// Mocks the MongoDb driver interfaces (client, database, collections and cursors) so the
/// data layer can be unit tested without a server. The filters sent to the collections are
/// captured and can be rendered to BsonDocument to assert the generated queries.
/// </summary>
public sealed class MongoMockContext
{
    public const string DatabaseName = "OrionTests";

    public Mock<IMongoClient> Client { get; } = new();
    public Mock<IMongoDatabase> Database { get; } = new();
    public Mock<IMongoCollection<User>> Users { get; } = new();
    public Mock<IMongoCollection<RefreshToken>> RefreshTokens { get; } = new();

    public MongoDbOptions Options { get; } = new()
    {
        ConnectionString = "mongodb://localhost:27017",
        DatabaseName = DatabaseName
    };

    public MongoMockContext()
    {
        MongoMappingConfiguration.Configure();

        Client.Setup(client => client.GetDatabase(DatabaseName, It.IsAny<MongoDatabaseSettings>()))
            .Returns(Database.Object);

        Database.Setup(database => database.DatabaseNamespace)
            .Returns(new DatabaseNamespace(DatabaseName));

        Database.Setup(database => database.GetCollection<User>(nameof(User), It.IsAny<MongoCollectionSettings>()))
            .Returns(Users.Object);

        Database.Setup(database => database.GetCollection<RefreshToken>(nameof(RefreshToken), It.IsAny<MongoCollectionSettings>()))
            .Returns(RefreshTokens.Object);
    }

    public MongoDataContext CreateDataContext()
    {
        return new MongoDataContext(Client.Object, Options);
    }

    /// <summary>
    /// Makes every Find of the collection return the given documents, capturing the filter and options used.
    /// </summary>
    public static FindCapture<T> SetupFind<T>(Mock<IMongoCollection<T>> collection, params T[] documents)
    {
        var capture = new FindCapture<T>();

        collection.Setup(current => current.FindAsync(
                It.IsAny<FilterDefinition<T>>(),
                It.IsAny<FindOptions<T, T>>(),
                It.IsAny<CancellationToken>()))
            .Callback<FilterDefinition<T>, FindOptions<T, T>, CancellationToken>((filter, options, _) =>
            {
                capture.Filter = filter;
                capture.Options = options;
            })
            .ReturnsAsync(() => CreateCursor(documents));

        return capture;
    }

    public static IAsyncCursor<T> CreateCursor<T>(params T[] documents)
    {
        var cursor = new Mock<IAsyncCursor<T>>();

        cursor.SetupSequence(current => current.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        cursor.SetupSequence(current => current.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

        cursor.Setup(current => current.Current).Returns(documents.ToList());

        return cursor.Object;
    }

    public static BsonDocument Render<T>(FilterDefinition<T> filter)
    {
        return filter.Render(CreateRenderArgs<T>());
    }

    public static BsonDocument Render<T>(SortDefinition<T> sort)
    {
        return sort.Render(CreateRenderArgs<T>());
    }

    public static BsonDocument Render<T>(IndexKeysDefinition<T> keys)
    {
        return keys.Render(CreateRenderArgs<T>());
    }

    private static RenderArgs<T> CreateRenderArgs<T>()
    {
        return new RenderArgs<T>(BsonSerializer.LookupSerializer<T>(), BsonSerializer.SerializerRegistry);
    }
}

public sealed class FindCapture<T>
{
    public FilterDefinition<T> Filter { get; set; }

    public FindOptions<T, T> Options { get; set; }

    public BsonDocument RenderedFilter => MongoMockContext.Render(Filter);

    public IEnumerable<string> FilterFields => RenderedFilter.Names;
}
