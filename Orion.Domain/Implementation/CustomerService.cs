using System.Threading.Tasks;
using Orion.Domain.Exceptions;
using Orion.Domain.Interfaces;
using Orion.Domain.Repositories.UnitOfWork;
using Orion.Entities.Domain;
using Orion.Entities.Filter;
using Orion.Entities.ValueObjects.Pagination;

namespace Orion.Domain.Implementation
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

        public async Task DeleteAsync(string publicId)
        {
            var item = await FindByIdAsync(publicId);

            if (item == null)
            {
                throw new NotFoundException(publicId);
            }

            await _unitOfWork.CustomerRepository.DeleteAsync(publicId);
            await _unitOfWork.CommitAsync();
        }

        public async Task<Customer> FindByIdAsync(string publicId)
        {
            return await _unitOfWork.CustomerRepository.FindByIdAsync(publicId);
        }

        public async Task<PagedList<Customer>> ListPaginate(CustomerFilter filter)
        {
            return await _unitOfWork.CustomerRepository.ListPaginate(filter);
        }

        public async Task UpdateAsync(Customer entity)
        {
            var entitySaved = await FindByIdAsync(entity.PublicId);

            entitySaved.Name = entity.Name;
            entitySaved.PublicId = entity.PublicId;

            _unitOfWork.CustomerRepository.Update(entitySaved);
            await _unitOfWork.CommitAsync();
        }
    }
}
