using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Invest.Data.Repository.Generic
{
    public interface IBaseEntityRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task AddRange(IEnumerable<T> listEntity);
        Task DeleteAsync(string publicId);
        Task<IEnumerable<T>> GetBy(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAll();
        Task<T> FindByIdAsync(string publicId);
        void Update(T entity);
    }
}
