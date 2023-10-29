using MediatR;

namespace Orion.Application.Core.Commands.CustomerDelete;

public class CustomerDeleteCommand : IRequest <Unit>
{
    public CustomerDeleteCommand(string publicId)
    {
        PublicId = publicId;
    }

    public string PublicId { get; private set; }
}

