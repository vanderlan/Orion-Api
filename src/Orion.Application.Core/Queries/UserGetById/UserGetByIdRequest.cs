using MediatR;

namespace Orion.Application.Core.Queries.UserGetById;

public class UserGetByIdRequest : IRequest <UserGetByIdResponse>
{
    public UserGetByIdRequest(string publicId)
    {
        PublicId = publicId;
    }

    public string PublicId { get; private set; }
}
