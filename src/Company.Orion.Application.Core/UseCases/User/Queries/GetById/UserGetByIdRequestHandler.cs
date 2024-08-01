using MediatR;
using Company.Orion.Domain.Core.Services.Interfaces;

namespace Company.Orion.Application.Core.UseCases.User.Queries.GetById;

public class UserGetByIdHandler(IUserService userService) : IRequestHandler<UserGetByIdRequest, UserGetByIdResponse>
{
    public async Task<UserGetByIdResponse> Handle(UserGetByIdRequest request, CancellationToken cancellationToken)
    {
        return (UserGetByIdResponse)await userService.FindByIdAsync(request.PublicId);
    }
}