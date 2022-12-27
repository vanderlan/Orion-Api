using System.Threading.Tasks;
using VBaseProject.Domain.Exceptions;
using VBaseProject.Domain.Interfaces;
using VBaseProject.Domain.Repositories.UnitOfWork;
using VBaseProject.Entities.Domain;
using VBaseProject.Entities.Filter;
using VBaseProject.Entities.ValueObjects.Pagination;

namespace VBaseProject.Domain.Implementation
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
