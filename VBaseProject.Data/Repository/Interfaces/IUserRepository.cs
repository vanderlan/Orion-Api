using System.Threading.Tasks;
using VBaseProject.Data.Repository.Generic;
using VBaseProject.Entities.Domain;

namespace VBaseProject.Data.Repository.Interfaces
{
    public interface IUserRepository : IBaseEntityRepository<User>
    {
        Task<User> LoginAsync(string email, string password);
    }
}
