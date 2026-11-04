using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Repositories.Base;
using Company.Orion.Infra.Data.Context;

namespace Company.Orion.Infra.Data.Repository.Generic;

public abstract class BaseEntityRepository<T>(DataContext dataContext) : IBaseEntityRepository<T>
    where T : BaseEntity
{
    protected DataContext DataContext { get; } = dataContext;

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var added = await DataContext.Set<T>().AddAsync(entity, cancellationToken);

        return added.Entity;
    }

    public virtual async Task DeleteAsync(string publicId, CancellationToken cancellationToken = default)
    {
        //An instance already tracked by the context (e.g. updated earlier in the same unit of work)
        //must be the one removed, otherwise EF would see two instances with the same key.
        var tracked = DataContext.Set<T>().Local.FirstOrDefault(entity => entity.PublicId == publicId);

        if (tracked is not null)
        {
            DataContext.Set<T>().Remove(tracked);
            return;
        }

        var existing = await GetByIdAsync(publicId, cancellationToken);

        if (existing is not null)
        {
            DataContext.Set<T>().Remove(existing);
        }
    }

    public virtual async Task<T> GetByIdAsync(string publicId, CancellationToken cancellationToken = default)
    {
        return await DataContext.Set<T>().AsNoTracking().SingleOrDefaultAsync(x => x.PublicId == publicId, cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> SearchByAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DataContext.Set<T>().AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual void Update(T entity)
    {
        DataContext.Set<T>().Update(entity);
    }
}
