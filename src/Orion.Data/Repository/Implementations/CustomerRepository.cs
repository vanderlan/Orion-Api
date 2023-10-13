using Orion.Data.Context;
using Orion.Data.Repository.Generic;
using Orion.Domain.Repositories;
using Orion.Entities.Domain;
using Orion.Entities.Filter;
using System.Linq;

namespace Orion.Data.Repository.Implementations
{
    internal class CustomerRepository : BaseEntityRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(DataContext context) : base(context)
        {

        }

        protected override IQueryable<Customer> ApplyFilters(BaseFilter<Customer> filter, IQueryable<Customer> query)
        {
            if (!string.IsNullOrWhiteSpace(filter.Query))
                query = query.Where(x => x.Name.Contains(filter.Query));

            return query;
        }
    }
}

