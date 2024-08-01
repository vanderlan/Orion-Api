using Company.Orion.Domain.Core.Services.Interfaces;
using MediatR;

namespace Company.Orion.Application.Core.UseCases.Users.Queries.GetById;

public class UserGetByIdHandler(IUserService userService) : IRequestHandler<UserGetByIdRequest, UserGetByIdResponse>
{
    public async Task<UserGetByIdResponse> Handle(UserGetByIdRequest request, CancellationToken cancellationToken)
    {
        return (UserGetByIdResponse)await userService.FindByIdAsync(request.PublicId);
    }
}