using MediatR;
using Company.Orion.Domain.Core.Services.Interfaces;

namespace Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithRefreshToken;

public class LoginWithRefreshTokenRequestHandler(IUserService userService)
    : IRequestHandler<LoginWithRefreshTokenRequest, LoginWithRefreshTokenResponse>
{
    public async Task<LoginWithRefreshTokenResponse> Handle(LoginWithRefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var (user, refreshToken) = await userService.SignInWithRefreshTokenAsync(request.RefreshToken, request.Token);

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