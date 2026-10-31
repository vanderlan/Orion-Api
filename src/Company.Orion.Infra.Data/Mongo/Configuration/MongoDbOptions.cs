using System;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Company.Orion.Infra.Data.Mongo.Configuration;

/// <summary>
/// Holds the MongoDb settings used by the application.
/// The connection string is read from <c>ConnectionStrings:MongoDb</c> and the database
/// name from <c>MongoDb:DatabaseName</c> (falling back to the database informed on the connection string).
/// </summary>
public sealed class MongoDbOptions
{
    public const string ConnectionStringPath = "ConnectionStrings:MongoDb";
    public const string DatabaseNamePath = "MongoDb:DatabaseName";

    public required string ConnectionString { get; init; }

    public required string DatabaseName { get; init; }

    public static MongoDbOptions FromConfiguration(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetSection(ConnectionStringPath).Value;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"The MongoDb connection string was not found. Set it on the '{ConnectionStringPath}' configuration.");
        }

        var databaseName = configuration.GetSection(DatabaseNamePath).Value;

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            databaseName = MongoUrl.Create(connectionString).DatabaseName;
        }

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException(
                $"The MongoDb database name was not found. Set it on the '{DatabaseNamePath}' configuration or on the connection string.");
        }

        return new MongoDbOptions
        {
            ConnectionString = connectionString,
            DatabaseName = databaseName
        };
    }
}
