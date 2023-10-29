using MediatR;

namespace Orion.Application.Core.Queries.CustomerGetById;

public class CustomerGetByIdQuery : IRequest <CustomerGetByIdResponse>
{
    public CustomerGetByIdQuery(string publicId)
    {
        PublicId = publicId;
    }

    public string PublicId { get; private set; }
}
