using System;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Entities.Enuns;
using Company.Orion.Infra.Data.Mongo.Mapping;
using Company.Orion.Test.Shared.Faker;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Xunit;

namespace Company.Orion.Test.Unit.Infra.Mongo;

public class MongoMappingConfigurationTests
{
    public MongoMappingConfigurationTests()
    {
        MongoMappingConfiguration.Configure();
    }

    [Fact]
    public void GetCollectionName_ReturnsTheEntityName()
    {
        //arrange & act & assert
        Assert.Equal(nameof(User), MongoMappingConfiguration.GetCollectionName<User>());
        Assert.Equal(nameof(RefreshToken), MongoMappingConfiguration.GetCollectionName<RefreshToken>());
    }

    [Theory]
    [InlineData(typeof(User))]
    [InlineData(typeof(RefreshToken))]
    public void Configure_MapsThePublicIdAsTheDocumentIdentifier(Type entityType)
    {
        //arrange & act
        var classMap = BsonClassMap.LookupClassMap(entityType);

        //assert
        Assert.NotNull(classMap.IdMemberMap);
        Assert.Equal(nameof(BaseEntity.PublicId), classMap.IdMemberMap.MemberName);
    }

    [Fact]
    public void Configure_DoesNotMapTheRelationalUserIdentifier()
    {
        //arrange
        var user = UserFaker.Get();

        //act
        var document = user.ToBsonDocument();

        //assert
        Assert.False(document.Contains(nameof(User.UserId)));
        Assert.Equal(user.PublicId, document["_id"].AsString);
    }

    [Fact]
    public void Configure_SerializesAndDeserializesTheUser()
    {
        //arrange
        var user = UserFaker.Get();
        user.CreatedAt = DateTime.UtcNow;
        user.LastUpdated = DateTime.UtcNow;

        //act
        var deserialized = BsonSerializer.Deserialize<User>(user.ToBsonDocument());

        //assert
        Assert.Equal(user.PublicId, deserialized.PublicId);
        Assert.Equal(user.Name, deserialized.Name);
        Assert.Equal(user.Email, deserialized.Email);
        Assert.Equal(user.Password, deserialized.Password);
        Assert.Equal(user.Profile, deserialized.Profile);
    }

    [Fact]
    public void Configure_CanBeCalledMultipleTimes()
    {
        //arrange & act
        var exception = Record.Exception(() =>
        {
            MongoMappingConfiguration.Configure();
            MongoMappingConfiguration.Configure();
        });

        //assert
        Assert.Null(exception);
        Assert.True(BsonClassMap.IsClassMapRegistered(typeof(BaseEntity)));
        Assert.True(BsonClassMap.IsClassMapRegistered(typeof(User)));
        Assert.True(BsonClassMap.IsClassMapRegistered(typeof(RefreshToken)));
    }

    [Theory]
    [InlineData(typeof(User))]
    [InlineData(typeof(RefreshToken))]
    public void Configure_IgnoresExtraElementsOfTheDocuments(Type entityType)
    {
        //arrange
        var classMap = BsonClassMap.LookupClassMap(entityType);

        //assert
        Assert.True(classMap.IgnoreExtraElements);
    }

    [Fact]
    public void Configure_DeserializesDocumentsWithUnknownFields()
    {
        //arrange
        var user = UserFaker.Get();
        var document = user.ToBsonDocument();
        document["LegacyField"] = "value that does not exist on the entity";

        //act
        var deserialized = BsonSerializer.Deserialize<User>(document);

        //assert
        Assert.Equal(user.PublicId, deserialized.PublicId);
        Assert.Equal(user.Email, deserialized.Email);
    }

    [Fact]
    public void Configure_StoresTheProfileAsIntegerLikeTheRelationalProviders()
    {
        //arrange
        var user = UserFaker.Get();
        user.Profile = UserProfile.Operator;

        //act
        var document = user.ToBsonDocument();

        //assert
        Assert.Equal(BsonType.Int32, document[nameof(User.Profile)].BsonType);
        Assert.Equal((int)UserProfile.Operator, document[nameof(User.Profile)].AsInt32);
    }

    [Fact]
    public void Configure_StoresTheTimestampsAsUtcDates()
    {
        //arrange
        var user = UserFaker.Get();
        user.CreatedAt = new DateTime(2025, 1, 2, 3, 4, 5, 678, DateTimeKind.Utc);
        user.LastUpdated = user.CreatedAt;

        //act
        var document = user.ToBsonDocument();
        var deserialized = BsonSerializer.Deserialize<User>(document);

        //assert
        Assert.Equal(BsonType.DateTime, document[nameof(User.CreatedAt)].BsonType);
        Assert.Equal(user.CreatedAt, deserialized.CreatedAt);
        Assert.Equal(DateTimeKind.Utc, deserialized.CreatedAt.Kind);
        Assert.Equal(user.LastUpdated, deserialized.LastUpdated);
    }

    [Fact]
    public void Configure_SerializesAndDeserializesTheRefreshToken()
    {
        //arrange
        var refreshToken = RefreshTokenFaker.Get();

        //act
        var document = refreshToken.ToBsonDocument();
        var deserialized = BsonSerializer.Deserialize<RefreshToken>(document);

        //assert
        Assert.Equal(refreshToken.PublicId, document["_id"].AsString);
        Assert.Equal(refreshToken.PublicId, deserialized.PublicId);
        Assert.Equal(refreshToken.Refreshtoken, deserialized.Refreshtoken);
        Assert.Equal(refreshToken.Email, deserialized.Email);
    }
}
