using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Repositories.Base;
using Company.Orion.Infra.Data.Mongo.Context;
using MongoDB.Driver;

namespace Company.Orion.Infra.Data.Mongo.Repository.Generic;

public abstract class MongoBaseEntityRepository<T>(MongoDataContext dataContext) : IBaseEntityRepository<T>
    where T : BaseEntity
{
    protected MongoDataContext DataContext { get; } = dataContext;

    protected IMongoCollection<T> Collection => DataContext.GetCollection<T>();

    public virtual Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        DataContext.Insert(entity);

        return Task.FromResult(entity);
    }

    public virtual async Task DeleteAsync(string publicId, CancellationToken cancellationToken = default)
    {
        var existing = await GetByIdAsync(publicId, cancellationToken);

        if (existing != null)
        {
            DataContext.Delete(existing);
        }
    }

    public virtual async Task<T> GetByIdAsync(string publicId, CancellationToken cancellationToken = default)
    {
        return await Collection.Find(entity => entity.PublicId == publicId).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> SearchByAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await Collection.Find(predicate).ToListAsync(cancellationToken);
    }

    public virtual void Update(T entity)
    {
        DataContext.Replace(entity);
    }
}
