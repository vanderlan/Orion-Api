using System.Threading.Tasks;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Filters;
using Company.Orion.Domain.Core.Services.Interfaces.Base;
using Company.Orion.Domain.Core.ValueObjects.Pagination;

namespace Company.Orion.Domain.Core.Services.Interfaces;

public interface IUserService : IBaseService<User>
{
    Task<(User User, RefreshToken RefreshToken)> SignInWithCredentialsAsync(string email, string password);
    Task<(User User, RefreshToken RefreshToken)> SignInWithRefreshTokenAsync(string refreshToken, string expiredToken);
    Task<PagedList<User>> ListPaginateAsync(UserFilter filter);
    Task ChangePasswordAsync(string userId, string currentPassword, string newPassword);
}
