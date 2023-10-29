using MediatR;
using Orion.Domain.Core.Services.Interfaces;

namespace Orion.Application.Core.Commands.UserCreate;

public class UserCreateRequestHandler : IRequestHandler<UserCreateRequest, UserCreateResponse>
{
    private readonly IUserService _userService;
    
    public UserCreateRequestHandler(IUserService userService)
    {
        _userService = userService;
    }
    
    public async Task<UserCreateResponse> Handle(UserCreateRequest request, CancellationToken cancellationToken)
    {
        var customerCreated = await _userService.AddAsync(request);

        return (UserCreateResponse)customerCreated;
    }
}