using System.Threading.Tasks;
using VBaseProject.Domain.Repositories.Base;
using VBaseProject.Entities.Domain;
using VBaseProject.Entities.Filter;
using VBaseProject.Entities.ValueObjects.Pagination;

namespace VBaseProject.Domain.Repositories
{
    public interface ICustomerRepository : IBaseEntityRepository<Customer>
    {
        Task<PagedList<Customer>> ListPaginate(CustomerFilter filter);
    }
}
