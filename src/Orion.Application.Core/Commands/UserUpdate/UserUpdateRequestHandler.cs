using MediatR;
using Orion.Domain.Core.Services.Interfaces;

namespace Orion.Application.Core.Commands.UserUpdate;

public class UserUpdateRequestHandler(IUserService userService) : IRequestHandler<UserUpdateRequest, Unit>
{
    public async Task<Unit> Handle(UserUpdateRequest request, CancellationToken cancellationToken)
    {
        await userService.UpdateAsync(request);
        
        return Unit.Value;
    }
}