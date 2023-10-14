using System.Threading.Tasks;
using Orion.Domain.Entities.Filter;
using Orion.Domain.Entities.ValueObjects.Pagination;
using Orion.Domain.Services.Interfaces.Base;
using Orion.Domain.Entities;

namespace Orion.Domain.Services.Interfaces;

public interface IUserService : IBaseService<User>
{
    Task<User> LoginAsync(string email, string password);
    Task<RefreshToken> AddRefreshTokenAsync(RefreshToken refreshToken);
    Task<User> GetUserByRefreshTokenAsync(string refreshToken);
    Task<PagedList<User>> ListPaginateAsync(BaseFilter<User> filter);
}
