using Company.Orion.Api.OpenApi.Examples.Shared;
using Company.Orion.Application.Core.UseCases.Users.Commands.Create;
using Company.Orion.Domain.Core.Entities.Enuns;

namespace Company.Orion.Api.OpenApi.Examples.Users;

public sealed class UserCreateResponseExample : IExampleProvider<UserCreateResponse>
{
    public UserCreateResponse GetExample()
    {
        return new UserCreateResponse
        {
            PublicId = ExampleData.AdminPublicId,
            Name = "Orion Admin User",
            Email = ExampleData.AdminEmail,
            Profile = UserProfile.Admin,
            CreatedAt = ExampleData.CreatedAt,
            LastUpdated = ExampleData.CreatedAt
        };
    }
}
