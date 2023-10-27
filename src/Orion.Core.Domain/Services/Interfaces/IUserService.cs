using System.Threading.Tasks;
using Orion.Core.Domain.Entities;
using Orion.Core.Domain.Entities.Filter;
using Orion.Core.Domain.Entities.ValueObjects.Pagination;
using Orion.Core.Domain.Services.Interfaces.Base;

namespace Orion.Core.Domain.Services.Interfaces;

public interface IUserService : IBaseService<User>
{
    Task<User> LoginAsync(string email, string password);
    Task<RefreshToken> AddRefreshTokenAsync(RefreshToken refreshToken);
    Task<User> SignInWithRehreshTokenAsync(string refreshToken, string expiredToken);
    Task<PagedList<User>> ListPaginateAsync(BaseFilter<User> filter);
}
