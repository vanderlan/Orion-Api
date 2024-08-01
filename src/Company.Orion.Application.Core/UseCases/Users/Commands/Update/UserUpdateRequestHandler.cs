using Company.Orion.Domain.Core.Services.Interfaces;
using MediatR;

namespace Company.Orion.Application.Core.UseCases.Users.Commands.Update;

public class UserUpdateRequestHandler(IUserService userService) : IRequestHandler<UserUpdateRequest>
{
    public async Task Handle(UserUpdateRequest request, CancellationToken cancellationToken)
    {
        await userService.UpdateAsync(request);
    }
}