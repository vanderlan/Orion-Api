using MediatR;
using Orion.Domain.Core.Entities;

namespace Orion.Application.Core.Commands.CustomerCreate;

public class CustomerCreateCommand : IRequest <CustomerCreateResponse>
{
    public string Name { get; set; }

    public static implicit operator Customer(CustomerCreateCommand command)
    {
        if (command is null)
            return default;
        
        return new Customer
        {
            Name = command.Name
        };
    }
}

