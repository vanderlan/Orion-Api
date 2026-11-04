using Company.Orion.Api.OpenApi.Examples.Shared;
using Company.Orion.Application.Core.UseCases.Users.Commands.Create;
using Company.Orion.Domain.Core.Entities.Enuns;

namespace Company.Orion.Api.OpenApi.Examples.Users;

/// <summary>
/// Multiple examples: the API reference shows a dropdown so the caller can pick the scenario to try.
/// </summary>
public sealed class UserCreateRequestExample : IMultipleExamplesProvider<UserCreateRequest>
{
    public IEnumerable<NamedExample<UserCreateRequest>> GetExamples()
    {
        yield return new NamedExample<UserCreateRequest>(
            "admin",
            new UserCreateRequest
            {
                Name = "Jane Admin",
                Email = "jane.admin@orion.com",
                Password = ExampleData.Password,
                Profile = UserProfile.Admin
            },
            Summary: "Admin user",
            Description: "Admins can manage users (full access to /Users).");

        yield return new NamedExample<UserCreateRequest>(
            "operator",
            new UserCreateRequest
            {
                Name = "Orion Operator",
                Email = ExampleData.OperatorEmail,
                Password = ExampleData.Password,
                Profile = UserProfile.Operator
            },
            Summary: "Operator user",
            Description: "Operators can authenticate but cannot manage users.");
    }
}
