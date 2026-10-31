using System;
using System.Collections.Generic;
using Company.Orion.Infra.Data.Mongo.Configuration;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Company.Orion.Test.Unit.Infra.Mongo;

public class MongoDbOptionsTests
{
    [Fact]
    public void FromConfiguration_WithConnectionStringAndDatabaseName_ReturnsOptions()
    {
        //arrange
        var configuration = BuildConfiguration(new Dictionary<string, string>
        {
            [MongoDbOptions.ConnectionStringPath] = "mongodb://localhost:27017",
            [MongoDbOptions.DatabaseNamePath] = "OrionDev"
        });

        //act
        var options = MongoDbOptions.FromConfiguration(configuration);

        //assert
        Assert.Equal("mongodb://localhost:27017", options.ConnectionString);
        Assert.Equal("OrionDev", options.DatabaseName);
    }

    [Fact]
    public void FromConfiguration_WithoutDatabaseName_UsesTheDatabaseOfTheConnectionString()
    {
        //arrange
        var configuration = BuildConfiguration(new Dictionary<string, string>
        {
            [MongoDbOptions.ConnectionStringPath] = "mongodb://localhost:27017/OrionDev"
        });

        //act
        var options = MongoDbOptions.FromConfiguration(configuration);

        //assert
        Assert.Equal("OrionDev", options.DatabaseName);
    }

    [Fact]
    public void FromConfiguration_WithoutConnectionString_ThrowsInvalidOperationException()
    {
        //arrange
        var configuration = BuildConfiguration(new Dictionary<string, string>());

        //act
        var exception = Assert.Throws<InvalidOperationException>(() => MongoDbOptions.FromConfiguration(configuration));

        //assert
        Assert.Contains(MongoDbOptions.ConnectionStringPath, exception.Message);
    }

    [Fact]
    public void FromConfiguration_WithoutDatabaseNameAnywhere_ThrowsInvalidOperationException()
    {
        //arrange
        var configuration = BuildConfiguration(new Dictionary<string, string>
        {
            [MongoDbOptions.ConnectionStringPath] = "mongodb://localhost:27017"
        });

        //act
        var exception = Assert.Throws<InvalidOperationException>(() => MongoDbOptions.FromConfiguration(configuration));

        //assert
        Assert.Contains(MongoDbOptions.DatabaseNamePath, exception.Message);
    }

    private static IConfiguration BuildConfiguration(Dictionary<string, string> values)
    {
        return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
    }
}
