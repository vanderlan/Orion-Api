using Company.Orion.Api.OpenApi.Examples.Shared;
using Company.Orion.Application.Core.UseCases.Users.Queries.GetById;
using Company.Orion.Domain.Core.Entities.Enuns;

namespace Company.Orion.Api.OpenApi.Examples.Users;

public sealed class UserGetByIdResponseExample : IExampleProvider<UserGetByIdResponse>
{
    public UserGetByIdResponse GetExample()
    {
        return new UserGetByIdResponse
        {
            PublicId = ExampleData.AdminPublicId,
            Name = "Orion Admin User",
            Email = ExampleData.AdminEmail,
            Profile = UserProfile.Admin,
            CreatedAt = ExampleData.CreatedAt,
            LastUpdated = ExampleData.LastUpdated
        };
    }
}
