using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace VBaseProject.Data.Repository.Generic
{
    public interface IBaseEntityRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task AddRange(IEnumerable<T> listEntity);
        Task DeleteAsync(string publicId);
        Task<IEnumerable<T>> GetBy(Expression<Func<T, bool>> predicate);
        Task<T> FindByIdAsync(string publicId);
        void Update(T entity);
    }
}
