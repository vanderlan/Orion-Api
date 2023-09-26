using System.Threading.Tasks;
using Orion.Domain.Repositories.Base;
using Orion.Entities.Domain;
using Orion.Entities.Filter;
using Orion.Entities.ValueObjects.Pagination;

namespace Orion.Domain.Repositories
{
    public interface IUserRepository : IBaseEntityRepository<User>
    {
        Task<User> LoginAsync(string email, string password);
        Task<PagedList<User>> ListPaginateAsync(UserFilter filter);
        Task<User> FindByEmailAsync(string email);
    }
}
