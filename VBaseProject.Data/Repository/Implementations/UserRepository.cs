using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VBaseProject.Data.Context;
using VBaseProject.Data.Repository.Generic;
using VBaseProject.Data.Repository.Interfaces;
using VBaseProject.Entities.Domain;

namespace VBaseProject.Data.Repository.Implementations
{
    internal class UserRepository : BaseEntityRepository<User>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context)
        {
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            var user = await DataContext.Users.Where(x => x.Email.Equals(email) && x.Password.Equals(password)).FirstOrDefaultAsync();

            return user ?? null;
        }
    }
}
