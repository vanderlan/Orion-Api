using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Entities.Enuns;
using Company.Orion.Domain.Core.Extensions;
using Company.Orion.Infra.Data.Mongo.Context;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Company.Orion.Infra.Data.Mongo.Initialization;

/// <summary>
/// MongoDb does not have migrations: the indexes and the default data are created on
/// the application startup. Both operations are idempotent and safe to run on every start.
/// </summary>
public sealed class MongoDbInitializer(MongoDataContext dataContext, ILogger<MongoDbInitializer> logger)
{
    public const string DefaultSystemUserPublicId = "16537902-1ca7-49ca-82e5-0be137f9aeeb";

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Initializing the MongoDb database {DatabaseName}",
            dataContext.Database.DatabaseNamespace.DatabaseName);

        await CreateIndexesAsync(cancellationToken);
        await SeedDefaultSystemUserAsync(cancellationToken);

        logger.LogInformation("The MongoDb database was successfully initialized");
    }

    private async Task CreateIndexesAsync(CancellationToken cancellationToken)
    {
        var userIndexes = new List<CreateIndexModel<User>>
        {
            new(Builders<User>.IndexKeys.Ascending(user => user.Email),
                new CreateIndexOptions { Name = "IX_User_Email", Unique = true })
        };

        await dataContext.Users.Indexes.CreateManyAsync(userIndexes, cancellationToken);

        var refreshTokenIndexes = new List<CreateIndexModel<RefreshToken>>
        {
            new(Builders<RefreshToken>.IndexKeys.Ascending(refreshToken => refreshToken.Email),
                new CreateIndexOptions { Name = "IX_RefreshToken_Email", Unique = true }),
            new(Builders<RefreshToken>.IndexKeys.Ascending(refreshToken => refreshToken.Refreshtoken),
                new CreateIndexOptions { Name = "IX_RefreshToken_Refreshtoken", Unique = true })
        };

        await dataContext.RefreshTokens.Indexes.CreateManyAsync(refreshTokenIndexes, cancellationToken);
    }

    private async Task SeedDefaultSystemUserAsync(CancellationToken cancellationToken)
    {
        var defaultUserFilter = Builders<User>.Filter.Eq(user => user.PublicId, DefaultSystemUserPublicId);

        var defaultUsersCount = await dataContext.Users.CountDocumentsAsync(
            defaultUserFilter,
            new CountOptions { Limit = 1 },
            cancellationToken);

        if (defaultUsersCount > 0)
            return;

        var defaultSystemUser = new User
        {
            PublicId = DefaultSystemUserPublicId,
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            LastUpdated = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Email = "adm@orion.com",
            Name = "Orion Admin User",
            Profile = UserProfile.Admin,
            Password = "123".ToSha512()
        };

        try
        {
            await dataContext.Users.InsertOneAsync(defaultSystemUser, cancellationToken: cancellationToken);
        }
        catch (MongoWriteException exception) when (exception.WriteError?.Category == ServerErrorCategory.DuplicateKey)
        {
            //Another instance has already created the default user.
            logger.LogInformation("The default system user already exists and was not created again");
        }
    }
}
