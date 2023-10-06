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
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Customer> AddAsync(Customer entity)
        {
            using var unitOfWork = _unitOfWork;

            var added = await unitOfWork.CustomerRepository.AddAsync(entity);
            await unitOfWork.CommitAsync();

            return added;
        }

        public async Task DeleteAsync(string publicId)
        {
            using var unitOfWork = _unitOfWork;

            var item = await FindByIdAsync(publicId);

            if (item == null)
            {
                throw new NotFoundException(publicId);
            }

            await unitOfWork.CustomerRepository.DeleteAsync(publicId);
            await unitOfWork.CommitAsync();
        }

        public async Task<Customer> FindByIdAsync(string publicId)
        {
            return await _unitOfWork.CustomerRepository.GetByIdAsync(publicId);
        }

        public async Task<PagedList<Customer>> ListPaginateAsync(CustomerFilter filter)
        {
            return await _unitOfWork.CustomerRepository.ListPaginate(filter);
        }

        public async Task UpdateAsync(Customer entity)
        {
            using var unitOfWork = _unitOfWork;

            var entitySaved = await FindByIdAsync(entity.PublicId);

            entitySaved.Name = entity.Name;
            entitySaved.PublicId = entity.PublicId;

            unitOfWork.CustomerRepository.Update(entitySaved);
            await unitOfWork.CommitAsync();
        }
    }
}
