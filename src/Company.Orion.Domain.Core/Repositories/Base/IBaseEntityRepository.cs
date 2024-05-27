using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Company.Orion.Domain.Core.Repositories.Base;

public interface IBaseEntityRepository<T> where T : class
{
    Task<T> AddAsync(T entity);
    Task DeleteAsync(string publicId);
    Task<IEnumerable<T>> SearchByAsync(Expression<Func<T, bool>> predicate);
    Task<T> GetByIdAsync(string publicId);
    void Update(T entity);
}
