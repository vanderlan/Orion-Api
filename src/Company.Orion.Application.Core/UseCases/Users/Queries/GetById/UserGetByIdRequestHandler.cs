using Company.Orion.Domain.Core.Exceptions;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using MediatR;

namespace Company.Orion.Application.Core.UseCases.Users.Queries.GetById;

public class UserGetByIdHandler(IUnitOfWork unitOfWork) : IRequestHandler<UserGetByIdRequest, UserGetByIdResponse>
{
    public async Task<UserGetByIdResponse> Handle(UserGetByIdRequest request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(request.PublicId);

        if (user == null)
            throw new NotFoundException(request.PublicId);

        return (UserGetByIdResponse)user;
    }
}