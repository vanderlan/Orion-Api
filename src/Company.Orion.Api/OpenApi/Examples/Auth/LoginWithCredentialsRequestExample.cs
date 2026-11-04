using Company.Orion.Api.OpenApi.Examples.Shared;
using Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithCredentials;

namespace Company.Orion.Api.OpenApi.Examples.Auth;

public sealed class LoginWithCredentialsRequestExample : IExampleProvider<LoginWithCredentialsRequest>
{
    public LoginWithCredentialsRequest GetExample()
    {
        return new LoginWithCredentialsRequest
        {
            Email = ExampleData.AdminEmail,
            Password = ExampleData.AdminPassword
        };
    }
}
