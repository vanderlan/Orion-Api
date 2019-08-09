using VBaseProject.Data.Context;
using VBaseProject.Data.Repository.Generic;
using VBaseProject.Data.Repository.Interfaces;
using VBaseProject.Entities.Domain;

namespace VBaseProject.Data.Repository.Implementations
{
    internal class CustomerRepository : BaseEntityRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(DataContext context) : base(context)
        {
        }

    }
}
