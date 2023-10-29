using MediatR;
using Orion.Domain.Core.Services.Interfaces;

namespace Orion.Application.Core.Commands.CustomerDelete;

public class CustomerDeleteCommandHandler : IRequestHandler<CustomerDeleteCommand, Unit>
{
    private readonly ICustomerService _customerService;
    
    public CustomerDeleteCommandHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }
    
    public async Task<Unit> Handle(CustomerDeleteCommand command, CancellationToken cancellationToken)
    {
        await _customerService.DeleteAsync(command.PublicId);
        
        return Unit.Value;
    }
}