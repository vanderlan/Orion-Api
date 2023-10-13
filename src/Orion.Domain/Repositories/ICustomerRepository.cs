using Orion.Domain.Entities;
using Orion.Domain.Entities.Filter;
using Orion.Domain.Entities.ValueObjects.Pagination;
using Orion.Domain.Repositories.Base;
using System.Threading.Tasks;

namespace Orion.Domain.Repositories
{
    public interface ICustomerRepository : IBaseEntityRepository<Customer>
    {
        Task<PagedList<Customer>> ListPaginateAsync(BaseFilter<Customer> filter);
    }
}
