namespace Company.Orion.Infra.Data.Mongo.Context;

/// <summary>
/// The write operations supported by the <see cref="MongoDataContext"/>.
/// </summary>
public enum MongoWriteOperation
{
    Insert = 1,
    Replace = 2,
    Delete = 3
}
