using System.Threading.Tasks;
using VBaseProject.Domain.Repositories.Base;
using VBaseProject.Entities.Domain;
using VBaseProject.Entities.Filter;
using VBaseProject.Entities.ValueObjects.Pagination;

namespace VBaseProject.Domain.Repositories
{
    public interface IUserRepository : IBaseEntityRepository<User>
    {
        Task<User> LoginAsync(string email, string password);
        Task<PagedList<User>> ListPaginate(UserFilter filter);
        Task<User> FindByEmailAsync(string email);
    }
}
