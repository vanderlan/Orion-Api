using System.Threading.Tasks;
using Orion.Entities.Domain;
using Orion.Entities.Filter;
using Orion.Entities.ValueObjects.Pagination;
using Orion.Domain.Base;

namespace Orion.Domain.Interfaces
{
    public interface ICustomerService : IBaseService<Customer>
    {
        Task<PagedList<Customer>> ListPaginate(CustomerFilter filter);
    }
}
