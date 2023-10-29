using System.Threading.Tasks;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Filters;
using Orion.Domain.Core.ValueObjects.Pagination;
using Orion.Domain.Core.Repositories.Base;

namespace Orion.Domain.Core.Repositories;

public interface ICustomerRepository : IBaseEntityRepository<Customer>
{
    Task<PagedList<Customer>> ListPaginateAsync(BaseFilter<Customer> filter);
}
