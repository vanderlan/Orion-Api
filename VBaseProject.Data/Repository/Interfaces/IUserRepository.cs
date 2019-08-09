using VBaseProject.Data.Repository.Generic;
using VBaseProject.Entities.Domain;
using System.Threading.Tasks;

namespace VBaseProject.Data.Repository.Interfaces
{
    public interface IUserRepository : IBaseEntityRepository<User>
    {
        Task<User> LoginAsync(string email, string password);
    }
}
