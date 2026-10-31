using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Infra.Data.Mongo.Configuration;
using Company.Orion.Infra.Data.Mongo.Mapping;
using MongoDB.Driver;

namespace Company.Orion.Infra.Data.Mongo.Context;

/// <summary>
/// A unit of work aware wrapper of <see cref="IMongoDatabase"/>.
/// The write operations are kept in memory until <see cref="SaveChangesAsync"/> is called,
/// keeping the same behaviour of the Entity Framework implementation (CreatedAt, LastUpdated
/// and PublicId are assigned on save).
/// </summary>
public class MongoDataContext
{
    private readonly List<IMongoPendingWrite> _pendingWrites = [];

    public MongoDataContext(IMongoClient mongoClient, MongoDbOptions options)
    {
        ArgumentNullException.ThrowIfNull(mongoClient);
        ArgumentNullException.ThrowIfNull(options);

        MongoMappingConfiguration.Configure();

        Client = mongoClient;
        Database = mongoClient.GetDatabase(options.DatabaseName);
    }

    public IMongoClient Client { get; }

    public IMongoDatabase Database { get; }

    #region Collections
    public IMongoCollection<User> Users => GetCollection<User>();
    public IMongoCollection<RefreshToken> RefreshTokens => GetCollection<RefreshToken>();
    #endregion

    public bool HasPendingWrites => _pendingWrites.Count > 0;

    public IMongoCollection<T> GetCollection<T>() where T : BaseEntity
    {
        return Database.GetCollection<T>(MongoMappingConfiguration.GetCollectionName<T>());
    }

    public void Insert<T>(T entity) where T : BaseEntity
    {
        AddPendingWrite(entity, MongoWriteOperation.Insert);
    }

    public void Replace<T>(T entity) where T : BaseEntity
    {
        AddPendingWrite(entity, MongoWriteOperation.Replace);
    }

    public void Delete<T>(T entity) where T : BaseEntity
    {
        AddPendingWrite(entity, MongoWriteOperation.Delete);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (!HasPendingWrites)
            return 0;

        AddTimestamps();

        var affectedDocuments = 0;

        foreach (var pendingWrite in _pendingWrites)
        {
            affectedDocuments += await pendingWrite.ExecuteAsync(this, cancellationToken);
        }

        _pendingWrites.Clear();

        return affectedDocuments;
    }

    public void DiscardChanges()
    {
        _pendingWrites.Clear();
    }

    private void AddPendingWrite<T>(T entity, MongoWriteOperation operation) where T : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);

        _pendingWrites.Add(new MongoPendingWrite<T>(entity, operation));
    }

    private void AddTimestamps()
    {
        foreach (var pendingWrite in _pendingWrites)
        {
            if (pendingWrite.Operation == MongoWriteOperation.Delete)
                continue;

            if (pendingWrite.Operation == MongoWriteOperation.Insert)
            {
                pendingWrite.Entity.CreatedAt = UtcNow();
                pendingWrite.Entity.PublicId = Guid.NewGuid().ToString();
            }

            pendingWrite.Entity.LastUpdated = UtcNow();
        }
    }

    /// <summary>
    /// BSON dates have millisecond precision, so the timestamps are truncated to keep the
    /// in memory entity exactly equal to the persisted document.
    /// </summary>
    private static DateTime UtcNow()
    {
        var utcNow = DateTime.UtcNow;

        return new DateTime(utcNow.Ticks - (utcNow.Ticks % TimeSpan.TicksPerMillisecond), DateTimeKind.Utc);
    }
}
