using MediatR;
using Orion.Domain.Core.Services.Interfaces;

namespace Orion.Application.Core.Queries.UserGetById;

public class UserGetByIdHandler(IUserService userService) : IRequestHandler<UserGetByIdRequest, UserGetByIdResponse>
{
    public async Task<UserGetByIdResponse> Handle(UserGetByIdRequest request, CancellationToken cancellationToken)
    {
        return (UserGetByIdResponse) await userService.FindByIdAsync(request.PublicId);
    }
}