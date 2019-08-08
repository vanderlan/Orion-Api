using Invest.Data.Repository.Generic;
using Invest.Entities.Domain;
using System.Threading.Tasks;

namespace Invest.Data.Repository.Interfaces
{
    public interface IUserRepository : IBaseEntityRepository<User>
    {
        Task<User> LoginAsync(string email, string password);
    }
}
