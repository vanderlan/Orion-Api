using MediatR;

namespace Orion.Application.Core.Queries.UserGetById;

public class UserGetByIdRequest(string publicId) : IRequest<UserGetByIdResponse>
{
    public string PublicId { get; private set; } = publicId;
}
