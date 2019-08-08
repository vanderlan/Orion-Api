using Invest.Entities.Domain;
using Invest.Service.Base;
using System.Threading.Tasks;

namespace Invest.Service.Interfaces
{
    public interface IUserService : IBaseService<User>
    {
        Task<User> LoginAsync(string email, string password);
    }
}
