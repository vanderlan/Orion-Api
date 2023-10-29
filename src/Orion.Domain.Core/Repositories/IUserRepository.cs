using System.Threading.Tasks;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Filters;
using Orion.Domain.Core.Repositories.Base;
using Orion.Domain.Core.ValueObjects.Pagination;

namespace Orion.Domain.Core.Repositories;

public interface IUserRepository : IBaseEntityRepository<User>
{
    Task<User> LoginAsync(string email, string password);
    Task<PagedList<User>> ListPaginateAsync(BaseFilter<User> filter);
    Task<User> FindByEmailAsync(string email);
}
