using System.Threading.Tasks;
using Orion.Entities.Domain;
using Orion.Entities.Filter;
using Orion.Entities.ValueObjects.Pagination;
using Orion.Domain.Base;

namespace Orion.Domain.Interfaces
{
    public interface IUserService : IBaseService<User>
    {
        Task<User> LoginAsync(string email, string password);
        Task<RefreshToken> AddRefreshTokenAsync(RefreshToken refreshToken);
        Task<User> GetUserByRefreshTokenAsync(string refreshToken);
        Task<PagedList<User>> ListPaginateAsync(UserFilter filter);
    }
}
