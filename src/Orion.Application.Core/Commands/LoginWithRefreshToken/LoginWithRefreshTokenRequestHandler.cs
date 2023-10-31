using MediatR;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Extensions;
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
        var user = await _userService.SignInWithRehreshTokenAsync(request.RefreshToken, request.Token);

        var refreshToken = await _userService.AddRefreshTokenAsync(
            new RefreshToken
            {
                Email = user.Email,
                Refreshtoken = Guid.NewGuid().ToString().ToSha512()
            });

        return new LoginWithRefreshTokenResponse
        {
            Email = user.Email,
            Name = user.Name,
            PublicId = user.PublicId,
            Profile = user.Profile,
            RefreshToken = refreshToken.Refreshtoken
        };
    }
}