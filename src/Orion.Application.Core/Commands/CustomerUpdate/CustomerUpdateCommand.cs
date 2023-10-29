using MediatR;
using Orion.Domain.Core.Entities;

namespace Orion.Application.Core.Commands.CustomerUpdate;

public class CustomerUpdateCommand : IRequest <Unit>
{
    public string PublicId { get; set; }
    public string Name { get; set; }

    public static implicit operator Customer(CustomerUpdateCommand command)
    {
        if (command is null)
            return default;
        
        return new Customer
        {
            PublicId = command.PublicId,
            Name = command.Name
        };
    }
}

