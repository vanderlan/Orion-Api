using Company.Orion.Api.Models;
using Company.Orion.Api.OpenApi.Examples.Shared;

namespace Company.Orion.Api.OpenApi.Examples.Auth;

public sealed class UserApiTokenModelExample : IExampleProvider<UserApiTokenModel>
{
    public UserApiTokenModel GetExample()
    {
        return new UserApiTokenModel
        {
            Token = ExampleData.Jwt,
            RefreshToken = ExampleData.RefreshToken,
            Expiration = ExampleData.TokenExpiration
        };
    }
}
