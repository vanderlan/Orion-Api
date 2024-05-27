using System.Threading.Tasks;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Filters;
using Company.Orion.Domain.Core.Repositories.Base;
using Company.Orion.Domain.Core.ValueObjects.Pagination;

namespace Company.Orion.Domain.Core.Repositories;

public interface IUserRepository : IBaseEntityRepository<User>
{
    Task<User> LoginAsync(string email, string password);
    Task<PagedList<User>> ListPaginateAsync(UserFilter filter);
    Task<User> FindByEmailAsync(string email);
}
