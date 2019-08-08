using Invest.Data.Context;
using Invest.Data.Repository.Generic;
using Invest.Data.Repository.Interfaces;
using Invest.Entities.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Invest.Data.Repository.Implementations
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
