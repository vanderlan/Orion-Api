using System.Threading.Tasks;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Filters;
using Orion.Domain.Core.Services.Interfaces.Base;
using Orion.Domain.Core.ValueObjects.Pagination;

namespace Orion.Domain.Core.Services.Interfaces;

public interface IUserService : IBaseService<User>
{
    Task<User> LoginAsync(string email, string password);
    Task<RefreshToken> AddRefreshTokenAsync(RefreshToken refreshToken);
    Task<User> SignInWithRehreshTokenAsync(string refreshToken, string expiredToken);
    Task<PagedList<User>> ListPaginateAsync(UserFilter filter);
}
