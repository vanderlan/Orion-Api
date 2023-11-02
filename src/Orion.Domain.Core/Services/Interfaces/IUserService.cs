using System.Threading.Tasks;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Filters;
using Orion.Domain.Core.Services.Interfaces.Base;
using Orion.Domain.Core.ValueObjects.Pagination;

namespace Orion.Domain.Core.Services.Interfaces;

public interface IUserService : IBaseService<User>
{
    Task<(User User, RefreshToken RefreshToken)> SignInWithCredentialsAsync(string email, string password);
    Task<(User User, RefreshToken RefreshToken)> SignInWithRefreshTokenAsync(string refreshToken, string expiredToken);
    Task<PagedList<User>> ListPaginateAsync(UserFilter filter);
}
