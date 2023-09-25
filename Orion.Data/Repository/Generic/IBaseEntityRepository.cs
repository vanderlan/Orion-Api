using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Orion.Data.Repository.Generic
{
    public interface IBaseEntityRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task DeleteAsync(string publicId);
        Task<IEnumerable<T>> GetByAsync(Expression<Func<T, bool>> predicate);
        Task<T> FindByIdAsync(string publicId);
        void Update(T entity);
    }
}