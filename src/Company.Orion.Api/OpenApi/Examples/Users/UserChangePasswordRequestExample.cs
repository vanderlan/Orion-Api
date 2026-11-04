using Company.Orion.Api.OpenApi.Examples.Shared;
using Company.Orion.Application.Core.UseCases.Users.Commands.ChangePassword;

namespace Company.Orion.Api.OpenApi.Examples.Users;

public sealed class UserChangePasswordRequestExample : IExampleProvider<UserChangePasswordRequest>
{
    public UserChangePasswordRequest GetExample()
    {
        return new UserChangePasswordRequest
        {
            CurrentPassword = ExampleData.Password,
            NewPassword = "N3w#Passw0rd!",
            NewPasswordConfirm = "N3w#Passw0rd!"
        };
    }
}
