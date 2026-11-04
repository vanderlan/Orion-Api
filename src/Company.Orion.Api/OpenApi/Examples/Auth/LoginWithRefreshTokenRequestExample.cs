using Company.Orion.Api.OpenApi.Examples.Shared;
using Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithRefreshToken;

namespace Company.Orion.Api.OpenApi.Examples.Auth;

public sealed class LoginWithRefreshTokenRequestExample : IExampleProvider<LoginWithRefreshTokenRequest>
{
    public LoginWithRefreshTokenRequest GetExample()
    {
        return new LoginWithRefreshTokenRequest
        {
            Token = ExampleData.Jwt,
            RefreshToken = ExampleData.RefreshToken
        };
    }
}
