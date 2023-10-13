using System.Threading.Tasks;
using Orion.Domain.Repositories.Base;
using Orion.Entities.Domain;
using Orion.Entities.Filter;
using Orion.Entities.ValueObjects.Pagination;

namespace Orion.Domain.Repositories
{
    public interface ICustomerRepository : IBaseEntityRepository<Customer>
    {
        Task<PagedList<Customer>> ListPaginate(CustomerFilter filter);
    }
}
