using System.Threading.Tasks;
using VBaseProject.Entities.Domain;
using VBaseProject.Entities.Filter;
using VBaseProject.Entities.ValueObjects.Pagination;
using VBaseProject.Service.Base;

namespace VBaseProject.Service.Interfaces
{
    public interface ICustomerService : IBaseService<Customer>
    {
        Task<PagedList<Customer>> ListPaginate(CustomerFilter filter);
    }
}
