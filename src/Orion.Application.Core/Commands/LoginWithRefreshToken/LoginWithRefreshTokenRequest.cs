using MediatR;

namespace Orion.Application.Core.Commands.LoginWithRefreshToken;

public class LoginWithRefreshTokenRequest : IRequest <LoginWithRefreshTokenResponse>
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}

