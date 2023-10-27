using System.Threading.Tasks;
using Orion.Core.Domain.Entities;
using Orion.Core.Domain.Entities.Filter;
using Orion.Core.Domain.Entities.ValueObjects.Pagination;
using Orion.Core.Domain.Services.Interfaces.Base;

namespace Orion.Core.Domain.Services.Interfaces;

public interface ICustomerService : IBaseService<Customer>
{
    Task<PagedList<Customer>> ListPaginateAsync(BaseFilter<Customer> filter);
}
