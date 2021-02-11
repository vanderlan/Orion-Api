using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VBaseProject.Data.Context;
using VBaseProject.Entities.Domain;

namespace VBaseProject.Data.Repository.Generic
{
    public class BaseEntityRepository<T> : IBaseEntityRepository<T> where T : BaseEntity
    {
        protected DataContext DataContext { get; }

        public BaseEntityRepository(DataContext dataContext)
        {
            DataContext = dataContext;
        }

        public async Task<T> AddAsync(T entity)
        {
            var added = await DataContext.Set<T>().AddAsync(entity);

            return added.Entity;
        }

        public async Task DeleteAsync(string publicId)
        {
            var existing = await FindByIdAsync(publicId);
            if (existing != null)
            {
                DataContext.Set<T>().Remove(existing);
            }
        }

        public virtual async Task<T> FindByIdAsync(string publicId)
        {
            return await DataContext.Set<T>().FirstOrDefaultAsync(x => x.PublicId == publicId);
        }

        public async Task<IEnumerable<T>> GetBy(Expression<Func<T, bool>> predicate)
        {
            return await DataContext.Set<T>().Where(predicate).ToListAsync();
        }

        public void Update(T entity)
        {
            DataContext.Entry(entity).State = EntityState.Modified;
            DataContext.Set<T>().Update(entity);
        }
    }
}
