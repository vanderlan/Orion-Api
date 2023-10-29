using MediatR;
using Orion.Domain.Core.Services.Interfaces;

namespace Orion.Application.Core.Commands.CustomerCreate;

public class CustomerCreateCommandHandler : IRequestHandler<CustomerCreateCommand, CustomerCreateResponse>
{
    private readonly ICustomerService _customerService;
    
    public CustomerCreateCommandHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }
    
    public async Task<CustomerCreateResponse> Handle(CustomerCreateCommand command, CancellationToken cancellationToken)
    {
        var customerCreated = await _customerService.AddAsync(command);

        return (CustomerCreateResponse)customerCreated;
    }
}