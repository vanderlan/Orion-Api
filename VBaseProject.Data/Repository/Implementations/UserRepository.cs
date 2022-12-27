using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VBaseProject.Data.Context;
using VBaseProject.Data.Repository.Generic;
using VBaseProject.Domain.Repositories;
using VBaseProject.Entities.Domain;
using VBaseProject.Entities.Filter;
using VBaseProject.Entities.ValueObjects.Pagination;

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

        public async Task<PagedList<User>> ListPaginate(UserFilter filter)
        {
            var pagination = (filter.Page * filter.Quantity) - filter.Quantity;

            IQueryable<User> listQuerable = DataContext.Users;

            if (!string.IsNullOrWhiteSpace(filter.Query))
            {
                listQuerable = listQuerable.Where(x => x.Name.Contains(filter.Query));
            }
            if (!string.IsNullOrWhiteSpace(filter?.Entity?.Name))
            {
                listQuerable = listQuerable.Where(x => x.Name.Contains(filter.Entity.Name));
            }

            var customerList = await listQuerable.OrderBy(x => x.Name).Skip(pagination).Take(filter.Quantity).ToListAsync();

            return new PagedList<User>(customerList, listQuerable.Count());
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await DataContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        }
    }
}
