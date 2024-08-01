using MediatR;

namespace Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithRefreshToken;

public class LoginWithRefreshTokenRequest : IRequest<LoginWithRefreshTokenResponse>
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}

