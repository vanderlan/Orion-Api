using Company.Orion.Api.OpenApi.Examples.Shared;
using Company.Orion.Application.Core.UseCases.Users.Commands.Update;
using Company.Orion.Domain.Core.Entities.Enuns;

namespace Company.Orion.Api.OpenApi.Examples.Users;

public sealed class UserUpdateRequestExample : IExampleProvider<UserUpdateRequest>
{
    public UserUpdateRequest GetExample()
    {
        //PublicId comes from the route, so it is intentionally not part of the body example.
        return new UserUpdateRequest
        {
            Name = "Orion Operator (renamed)",
            Email = ExampleData.OperatorEmail,
            Profile = UserProfile.Operator
        };
    }
}
