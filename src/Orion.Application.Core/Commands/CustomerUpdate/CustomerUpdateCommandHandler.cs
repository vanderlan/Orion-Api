using MediatR;
using Orion.Domain.Core.Services.Interfaces;

namespace Orion.Application.Core.Commands.CustomerUpdate;

public class CustomerUpdateCommandHandler : IRequestHandler<CustomerUpdateCommand, Unit>
{
    private readonly ICustomerService _customerService;
    
    public CustomerUpdateCommandHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }
    
    public async Task<Unit> Handle(CustomerUpdateCommand command, CancellationToken cancellationToken)
    {
        await _customerService.UpdateAsync(command);
        
        return Unit.Value;
    }
}