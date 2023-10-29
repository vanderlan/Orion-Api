using System.Threading.Tasks;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Filters;
using Orion.Domain.Core.Services.Interfaces.Base;
using Orion.Domain.Core.ValueObjects.Pagination;

namespace Orion.Domain.Core.Services.Interfaces;

public interface ICustomerService : IBaseService<Customer>
{
    Task<PagedList<Customer>> ListPaginateAsync(BaseFilter<Customer> filter);
}
