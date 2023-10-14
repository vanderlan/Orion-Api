using Orion.Domain.Entities;
using Orion.Domain.Entities.Filter;
using Orion.Domain.Entities.ValueObjects.Pagination;
using Orion.Domain.Repositories.Base;
using System.Threading.Tasks;

namespace Orion.Domain.Repositories;

public interface IUserRepository : IBaseEntityRepository<User>
{
    Task<User> LoginAsync(string email, string password);
    Task<PagedList<User>> ListPaginateAsync(BaseFilter<User> filter);
    Task<User> FindByEmailAsync(string email);
}
