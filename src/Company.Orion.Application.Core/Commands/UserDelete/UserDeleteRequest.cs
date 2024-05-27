using MediatR;

namespace Company.Orion.Application.Core.Commands.UserDelete;

public class UserDeleteRequest(string publicId) : IRequest<Unit>
{
    public string PublicId { get; private set; } = publicId;
}

