using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Repositories;
using System.Linq;
using Orion.Domain.Core.Filters;
using Orion.Infra.Data.Context;
using Orion.Infra.Data.Repository.Generic;

namespace Orion.Infra.Data.Repository.Implementations;

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

