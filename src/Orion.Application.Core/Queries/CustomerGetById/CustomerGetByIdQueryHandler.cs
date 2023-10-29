using MediatR;
using Orion.Domain.Core.Services.Interfaces;

namespace Orion.Application.Core.Queries.CustomerGetById;

public class CustomerGetByIdHandler : IRequestHandler<CustomerGetByIdQuery, CustomerGetByIdResponse>
{
    private readonly ICustomerService _customerService;
    
    public CustomerGetByIdHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }
    
    public async Task<CustomerGetByIdResponse> Handle(CustomerGetByIdQuery command, CancellationToken cancellationToken)
    {
        return (CustomerGetByIdResponse) await _customerService.FindByIdAsync(command.PublicId);
    }
}