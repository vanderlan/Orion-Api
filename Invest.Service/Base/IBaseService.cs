using System.Collections.Generic;
using System.Threading.Tasks;

namespace Invest.Service.Base
{
    public interface IBaseService<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task DeleteAsync(string publicId);
        Task<T> FindByIdAsync(string publicId);
        Task UpdateAsync(T entity);
        Task<IEnumerable<T>> GetAll();
    }
}

