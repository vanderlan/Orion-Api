using System.Threading.Tasks;
using Orion.Entities.Domain;
using Orion.Entities.Filter;
using Orion.Entities.ValueObjects.Pagination;
using Orion.Domain.Services.Interfaces.Base;

namespace Orion.Domain.Services.Interfaces
{
    public interface ICustomerService : IBaseService<Customer>
    {
        Task<PagedList<Customer>> ListPaginateAsync(CustomerFilter filter);
    }
}
