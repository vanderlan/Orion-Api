using System.Linq.Expressions;

namespace Company.Orion.Domain.Core.Repositories.Base;

public interface IBaseEntityRepository<T> where T : class
{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(string publicId, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> SearchByAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(string publicId, CancellationToken cancellationToken = default);
    void Update(T entity);
}
