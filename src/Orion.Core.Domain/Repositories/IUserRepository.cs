using System.Threading.Tasks;
using Orion.Core.Domain.Entities;
using Orion.Core.Domain.Entities.Filter;
using Orion.Core.Domain.Entities.ValueObjects.Pagination;
using Orion.Core.Domain.Repositories.Base;

namespace Orion.Core.Domain.Repositories;

public interface IUserRepository : IBaseEntityRepository<User>
{
    Task<User> LoginAsync(string email, string password);
    Task<PagedList<User>> ListPaginateAsync(BaseFilter<User> filter);
    Task<User> FindByEmailAsync(string email);
}
