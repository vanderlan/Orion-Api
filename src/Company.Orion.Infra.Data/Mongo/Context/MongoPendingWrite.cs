using System;
using System.Threading;
using System.Threading.Tasks;
using Company.Orion.Domain.Core.Entities;
using MongoDB.Driver;

namespace Company.Orion.Infra.Data.Mongo.Context;

/// <summary>
/// A write operation that was requested by a repository and is waiting for the commit of the unit of work.
/// </summary>
internal interface IMongoPendingWrite
{
    BaseEntity Entity { get; }

    MongoWriteOperation Operation { get; }

    Task<int> ExecuteAsync(MongoDataContext context, CancellationToken cancellationToken);
}

internal sealed class MongoPendingWrite<T>(T entity, MongoWriteOperation operation) : IMongoPendingWrite
    where T : BaseEntity
{
    public BaseEntity Entity { get; } = entity;

    public MongoWriteOperation Operation { get; } = operation;

    public async Task<int> ExecuteAsync(MongoDataContext context, CancellationToken cancellationToken)
    {
        var collection = context.GetCollection<T>();
        var filter = Builders<T>.Filter.Eq(current => current.PublicId, entity.PublicId);

        switch (Operation)
        {
            case MongoWriteOperation.Insert:
                await collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
                return 1;

            case MongoWriteOperation.Replace:
                var replaceResult = await collection.ReplaceOneAsync(filter, entity, new ReplaceOptions(), cancellationToken);
                return replaceResult.IsAcknowledged ? (int)replaceResult.ModifiedCount : 0;

            case MongoWriteOperation.Delete:
                var deleteResult = await collection.DeleteOneAsync(filter, cancellationToken);
                return deleteResult.IsAcknowledged ? (int)deleteResult.DeletedCount : 0;

            default:
                throw new NotSupportedException($"The MongoDb write operation '{Operation}' is not supported.");
        }
    }
}
