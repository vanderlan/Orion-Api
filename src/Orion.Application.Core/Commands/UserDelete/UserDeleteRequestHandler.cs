using MediatR;
using Orion.Domain.Core.Services.Interfaces;

namespace Orion.Application.Core.Commands.UserDelete;

public class UserDeleteRequestHandler : IRequestHandler<UserDeleteRequest, Unit>
{
    private readonly IUserService _userService;
    
    public UserDeleteRequestHandler(IUserService userService)
    {
        _userService = userService;
    }
    
    public async Task<Unit> Handle(UserDeleteRequest request, CancellationToken cancellationToken)
    {
        await _userService.DeleteAsync(request.PublicId);
        
        return Unit.Value;
    }
}