using Company.Orion.Domain.Core.Entities;
using MongoDB.Bson.Serialization;

namespace Company.Orion.Infra.Data.Mongo.Mapping;

/// <summary>
/// Registers the BsonClassMaps of the domain entities. It is the MongoDb counterpart
/// of the Entity Framework Fluent API mappings and it is applied only once per process.
/// </summary>
public static class MongoMappingConfiguration
{
    private static readonly object SyncRoot = new();

    private static volatile bool _configured;

    public static void Configure()
    {
        if (_configured)
            return;

        lock (SyncRoot)
        {
            if (_configured)
                return;

            RegisterClassMaps();

            _configured = true;
        }
    }

    /// <summary>
    /// The collection of an entity uses the entity name, keeping the same
    /// naming convention used by the relational implementations.
    /// </summary>
    public static string GetCollectionName<T>() where T : BaseEntity => typeof(T).Name;

    private static void RegisterClassMaps()
    {
        //The PublicId is the natural identifier of every entity, so it is mapped as the document _id.
        BsonClassMap.TryRegisterClassMap<BaseEntity>(map =>
        {
            map.AutoMap();
            map.MapIdMember(entity => entity.PublicId);
            map.SetIgnoreExtraElements(true);
            map.SetIgnoreExtraElementsIsInherited(true);
        });

        BsonClassMap.TryRegisterClassMap<User>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);

            //UserId is a relational surrogate key and has no meaning on MongoDb.
            map.UnmapMember(user => user.UserId);
        });

        BsonClassMap.TryRegisterClassMap<RefreshToken>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);
        });
    }
}
