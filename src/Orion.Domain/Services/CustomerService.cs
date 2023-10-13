using System.Threading.Tasks;
using Orion.Domain.Exceptions;
using Orion.Domain.Services.Interfaces;
using Orion.Domain.Repositories.UnitOfWork;
using Orion.Domain.Entities;
using Orion.Domain.Entities.ValueObjects.Pagination;
using Orion.Domain.Entities.Filter;

namespace Orion.Domain.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
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
            return await _unitOfWork.CustomerRepository.GetByIdAsync(publicId);
        }

        public async Task<PagedList<Customer>> ListPaginateAsync(BaseFilter<Customer> filter)
        {
            return await _unitOfWork.CustomerRepository.ListPaginateAsync(filter);
        }

        public async Task UpdateAsync(Customer entity)
        {
            using var unitOfWork = _unitOfWork;

            var entitySaved = await FindByIdAsync(entity.PublicId);

            entitySaved.Name = entity.Name;
            entitySaved.PublicId = entity.PublicId;

            _unitOfWork.CustomerRepository.Update(entitySaved);
            await _unitOfWork.CommitAsync();
        }
    }
}
