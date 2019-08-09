using VBaseProject.Data.UnitOfWork;
using VBaseProject.Entities.Domain;
using VBaseProject.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<Customer>> GetAll()
        {
            return await unitOfWork.CustomerRepository.GetBy(x => x.CustomerId > 0);
        }

        public async Task UpdateAsync(Customer entity)
        {
            var entitySaved = await FindByIdAsync(entity.PublicId);

            entitySaved.Code = entity.Code;
            entitySaved.Description = entity.Description;
            entitySaved.StockExchangeId = entity.StockExchangeId;
            entitySaved.Price = entity.Price;
            entitySaved.CompanyId = entity.CompanyId;

            unitOfWork.CustomerRepository.Update(entitySaved);
            await unitOfWork.CommitAsync();
        }
    }
}
