using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Orion.Data.Context;
using Orion.Domain.Entities.Filter;
using Orion.Domain.Entities.ValueObjects.Pagination;
using Orion.Domain.Entities;

namespace Orion.Data.Repository.Generic;

internal abstract class BaseEntityRepository<T> : IBaseEntityRepository<T> where T : BaseEntity
{
    protected DataContext DataContext { get; }

    protected BaseEntityRepository(DataContext dataContext)
    {
        DataContext = dataContext;
    }

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
        DataContext.ChangeTracker.Clear();
        DataContext.Entry(entity).State = EntityState.Modified;
        DataContext.Set<T>().Update(entity);
    }

    public virtual async Task<PagedList<T>> ListPaginateAsync(BaseFilter<T> filter)
    {
        IQueryable<T> query = DataContext.Set<T>();

        query = ApplyFilters(filter, query);

        var pagination = (filter.Page * filter.Quantity) - filter.Quantity;

        var entityList = await query.OrderBy(x => x.CreatedAt)
            .AsNoTracking()
            .Skip(pagination)
            .Take(filter.Quantity)
            .ToListAsync();

        return new PagedList<T>(entityList, query.Count());
    }

    /// <summary>
    /// Each repository must implement its filter, but if the class does not need a custom filter, only pagination and the OrderBy pattern will be applied
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="query"></param>
    /// <returns>IQuerable with filters applied</returns>
    protected virtual IQueryable<T> ApplyFilters(BaseFilter<T> filter, IQueryable<T> query)
    {
        return query;
    }
}
