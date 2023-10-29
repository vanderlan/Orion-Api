using MediatR;
using Orion.Application.Core.Queries.CustomerGetById;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Services.Interfaces;
using Orion.Domain.Core.ValueObjects.Pagination;

namespace Orion.Application.Core.Queries.CustomerGetPaginated;

public class CustomerGetPaginatedRequestHandler : IRequestHandler<CustomerGetPaginatedRequest, PagedList<Customer>>
{
    private readonly ICustomerService _customerService;
    
    public CustomerGetPaginatedRequestHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }
    
    public async Task<CustomerGetByIdResponse> Handle(CustomerGetPaginatedRequest request, CancellationToken cancellationToken)
    {
        return (PagedList<Customer>) await _customerService.ListPaginateAsync(request);
    }
}