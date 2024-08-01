using MediatR;
using Company.Orion.Domain.Core.Services.Interfaces;

namespace Company.Orion.Application.Core.UseCases.User.Commands.Update;

public class UserUpdateRequestHandler(IUserService userService) : IRequestHandler<UserUpdateRequest>
{
    public async Task Handle(UserUpdateRequest request, CancellationToken cancellationToken)
    {
        await userService.UpdateAsync(request);
    }
}