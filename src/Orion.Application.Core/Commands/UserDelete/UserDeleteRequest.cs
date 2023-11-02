using MediatR;

namespace Orion.Application.Core.Commands.UserDelete;

public class UserDeleteRequest : IRequest <Unit>
{
    public UserDeleteRequest(string publicId)
    {
        PublicId = publicId;
    }

    public string PublicId { get; private set; }
}

