using System.Threading.Tasks;
using Orion.Domain.Entities.Filter;
using Orion.Domain.Entities.ValueObjects.Pagination;
using Orion.Domain.Services.Interfaces.Base;
using Orion.Domain.Entities;

namespace Orion.Domain.Services.Interfaces
{
    public interface ICustomerService : IBaseService<Customer>
    {
        Task<PagedList<Customer>> ListPaginateAsync(BaseFilter<Customer> filter);
    }
}
