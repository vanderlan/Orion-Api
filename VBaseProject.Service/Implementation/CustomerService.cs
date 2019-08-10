using System.Threading.Tasks;
using VBaseProject.Data.UnitOfWork;
using VBaseProject.Entities.Domain;
using VBaseProject.Entities.Filter;
using VBaseProject.Entities.ValueObjects.Pagination;
using VBaseProject.Service.Interfaces;

namespace VBaseProject.Service.Implementation
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWorkEntity unitOfWork;

        public CustomerService(IUnitOfWorkEntity unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Customer> AddAsync(Customer entity)
        {
            var added = await unitOfWork.CustomerRepository.AddAsync(entity);
            await unitOfWork.CommitAsync();

            return added;
        }

        public async Task DeleteAsync(string id)
        {
            await unitOfWork.CustomerRepository.DeleteAsync(id);
            await unitOfWork.CommitAsync();
        }

        public async Task<Customer> FindByIdAsync(string id)
        {
            return await unitOfWork.CustomerRepository.FindByIdAsync(id);
        }

        public async Task<PagedList<Customer>> ListPaginate(CustomerFilter filter)
        {
            return await unitOfWork.CustomerRepository.ListPaginate(filter);
        }

        public async Task UpdateAsync(Customer entity)
        {
            var entitySaved = await FindByIdAsync(entity.PublicId);

            entitySaved.Name = entity.Name;
            entitySaved.Address = entity.Address;
            entitySaved.PhoneNumber = entity.PhoneNumber;
            entitySaved.PublicId = entity.PublicId;

            unitOfWork.CustomerRepository.Update(entitySaved);
            await unitOfWork.CommitAsync();
        }
    }
}
