using System.Threading.Tasks;
using Orion.Core.Domain.Entities;
using Orion.Core.Domain.Entities.Filter;
using Orion.Core.Domain.Entities.ValueObjects.Pagination;
using Orion.Core.Domain.Repositories.Base;

namespace Orion.Core.Domain.Repositories;

public interface ICustomerRepository : IBaseEntityRepository<Customer>
{
    Task<PagedList<Customer>> ListPaginateAsync(BaseFilter<Customer> filter);
}
