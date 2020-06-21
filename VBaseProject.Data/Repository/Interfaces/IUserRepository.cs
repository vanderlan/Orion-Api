using System.Threading.Tasks;
using VBaseProject.Data.Repository.Generic;
using VBaseProject.Entities.Domain;
using VBaseProject.Entities.Filter;
using VBaseProject.Entities.ValueObjects.Pagination;

namespace VBaseProject.Data.Repository.Interfaces
{
    public interface IUserRepository : IBaseEntityRepository<User>
    {
        Task<User> LoginAsync(string email, string password);
        Task<PagedList<User>> ListPaginate(UserFilter filter);
        Task<User> FindByEmailAsync(string email);
    }
}
