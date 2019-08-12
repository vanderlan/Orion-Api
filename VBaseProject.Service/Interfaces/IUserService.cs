using System.Threading.Tasks;
using VBaseProject.Entities.Domain;
using VBaseProject.Service.Base;

namespace VBaseProject.Service.Interfaces
{
    public interface IUserService : IBaseService<User>
    {
        Task<User> LoginAsync(string email, string password);
        Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken);
        Task<User> GetUserByRefreshToken(string refresh);
    }
}
