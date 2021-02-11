using System.Threading.Tasks;
using VBaseProject.Data.UnitOfWork;
using VBaseProject.Entities.Domain;
using VBaseProject.Entities.Filter;
using VBaseProject.Entities.ValueObjects.Pagination;
using VBaseProject.Service.Exceptions;
using VBaseProject.Service.Interfaces;

namespace VBaseProject.Service.Implementation
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWorkEntity _unitOfWork;

        public CustomerService(IUnitOfWorkEntity unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Customer> AddAsync(Customer entity)
        {
            var added = await _unitOfWork.CustomerRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            return added;
        }

        public async Task DeleteAsync(string id)
        {
            var item = await FindByIdAsync(id);

            if (item == null)
            {
                throw new NotFoundException(id);
            }

            await _unitOfWork.CustomerRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
        }

        public async Task<Customer> FindByIdAsync(string id)
        {
            return await _unitOfWork.CustomerRepository.FindByIdAsync(id);
        }

        public async Task<PagedList<Customer>> ListPaginate(CustomerFilter filter)
        {
            return await _unitOfWork.CustomerRepository.ListPaginate(filter);
        }

        public async Task UpdateAsync(Customer entity)
        {
            var entitySaved = await FindByIdAsync(entity.PublicId);

            entitySaved.Name = entity.Name;
            entitySaved.Address = entity.Address;
            entitySaved.PhoneNumber = entity.PhoneNumber;
            entitySaved.PublicId = entity.PublicId;

            _unitOfWork.CustomerRepository.Update(entitySaved);
            await _unitOfWork.CommitAsync();
        }
    }
}
