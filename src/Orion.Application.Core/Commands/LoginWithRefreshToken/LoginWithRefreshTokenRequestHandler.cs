using MediatR;
using Orion.Domain.Core.Services.Interfaces;

namespace Orion.Application.Core.Commands.LoginWithRefreshToken;

public class LoginWithRefreshTokenRequestHandler : IRequestHandler<LoginWithRefreshTokenRequest, LoginWithRefreshTokenResponse>
{
    private readonly IUserService _userService;
    
    public LoginWithRefreshTokenRequestHandler(IUserService userService)
    {
        _userService = userService;
    }
    
    public async Task<LoginWithRefreshTokenResponse> Handle(LoginWithRefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var customerCreated = await _userService.SignInWithRehreshTokenAsync(request.RefreshToken, request.Token);

        return (LoginWithRefreshTokenResponse)customerCreated;
    }
}