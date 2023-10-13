using Microsoft.EntityFrameworkCore;
using Orion.Data.Context;
using Orion.Data.Repository.Generic;
using Orion.Domain.Entities;
using Orion.Domain.Repositories;
using Orion.Domain.Entities.Filter;
using System.Linq;
using System.Threading.Tasks;

namespace Orion.Data.Repository.Implementations
{
    internal class UserRepository : BaseEntityRepository<User>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context)
        {
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            var user = await DataContext.Users.AsNoTracking()
                .Where(x => x.Email.Equals(email) && x.Password.Equals(password))
                .FirstOrDefaultAsync();

            return user ?? null;
        }

        protected override IQueryable<User> ApplyFilters(BaseFilter<User> filter, IQueryable<User> query)
        {
            if (!string.IsNullOrWhiteSpace(filter.Query))
                query = query.Where(x => x.Name.Contains(filter.Query));

            return query;
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await DataContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email.Equals(email));
        }
    }
}
