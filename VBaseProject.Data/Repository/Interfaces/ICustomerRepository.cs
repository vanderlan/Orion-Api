using System.Threading.Tasks;
using VBaseProject.Data.Repository.Generic;
using VBaseProject.Entities.Domain;
using VBaseProject.Entities.Filter;
using VBaseProject.Entities.ValueObjects.Pagination;

namespace VBaseProject.Data.Repository.Interfaces
{
    public interface ICustomerRepository : IBaseEntityRepository<Customer>
    {
        Task<PagedList<Customer>> ListPaginate(CustomerFilter filter);
    }
}
