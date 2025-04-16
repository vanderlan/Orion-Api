using Microsoft.EntityFrameworkCore;
using Company.Orion.Domain.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Company.Orion.Domain.Core.Repositories.Base;
using Company.Orion.Infra.Data.Context;

namespace Company.Orion.Infra.Data.Repository.Generic;

public abstract class BaseEntityRepository<T>(DataContext dataContext) : IBaseEntityRepository<T>
    where T : BaseEntity
{
    protected DataContext DataContext { get; } = dataContext;

    public virtual async Task<T> AddAsync(T entity)
    {
        var added = await DataContext.Set<T>().AddAsync(entity);

        return added.Entity;
    }

    public virtual async Task DeleteAsync(string publicId)
    {
        var existing = await GetByIdAsync(publicId);

        if (existing != null)
        {
            DataContext.ChangeTracker.Clear();
            DataContext.Set<T>().Remove(existing);
        }
    }

    public virtual async Task<T> GetByIdAsync(string publicId)
    {
        return await DataContext.Set<T>().AsNoTracking().SingleOrDefaultAsync(x => x.PublicId == publicId);
    }

    public virtual async Task<IEnumerable<T>> SearchByAsync(Expression<Func<T, bool>> predicate)
    {
        return await DataContext.Set<T>().AsNoTracking().Where(predicate).ToListAsync();
    }

    public virtual void Update(T entity)
    {
        DataContext.Entry(entity).State = EntityState.Modified;
        DataContext.Set<T>().Update(entity);
    }
}
