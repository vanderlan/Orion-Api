using MediatR;
using Orion.Domain.Core.Services.Interfaces;

namespace Orion.Application.Core.Commands.UserUpdate;

public class UserUpdateRequestHandler : IRequestHandler<UserUpdateRequest, Unit>
{
    private readonly IUserService _userService;
    
    public UserUpdateRequestHandler(IUserService userService)
    {
        _userService = userService;
    }
    
    public async Task<Unit> Handle(UserUpdateRequest request, CancellationToken cancellationToken)
    {
        await _userService.UpdateAsync(request);
        
        return Unit.Value;
    }
}