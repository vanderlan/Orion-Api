using VBaseProject.Entities.Domain;
using VBaseProject.Service.Base;
using System.Threading.Tasks;

namespace VBaseProject.Service.Interfaces
{
    public interface IUserService : IBaseService<User>
    {
        Task<User> LoginAsync(string email, string password);
    }
}
