using Company.Orion.Domain.Core.Services.Interfaces;
using MediatR;

namespace Company.Orion.Application.Core.UseCases.Users.Commands.Delete;

public class UserDeleteRequestHandler(IUserService userService) : IRequestHandler<UserDeleteRequest>
{
    public async Task Handle(UserDeleteRequest request, CancellationToken cancellationToken)
    {
        await userService.DeleteAsync(request.PublicId);
    }
}