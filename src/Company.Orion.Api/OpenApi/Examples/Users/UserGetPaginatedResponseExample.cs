using Company.Orion.Api.OpenApi.Examples.Shared;
using Company.Orion.Application.Core.UseCases.Users.Queries.GetPaginated;
using Company.Orion.Domain.Core.Entities.Enuns;
using Company.Orion.Domain.Core.ValueObjects.Pagination;

namespace Company.Orion.Api.OpenApi.Examples.Users;

public sealed class UserGetPaginatedResponseExample : IExampleProvider<PagedList<UserGetPaginatedResponse>>
{
    public PagedList<UserGetPaginatedResponse> GetExample()
    {
        var items = new List<UserGetPaginatedResponse>
        {
            new()
            {
                PublicId = ExampleData.AdminPublicId,
                Name = "Orion Admin User",
                Email = ExampleData.AdminEmail,
                Profile = UserProfile.Admin,
                CreatedAt = ExampleData.CreatedAt,
                LastUpdated = ExampleData.LastUpdated
            },
            new()
            {
                PublicId = ExampleData.OperatorPublicId,
                Name = "Orion Operator",
                Email = ExampleData.OperatorEmail,
                Profile = UserProfile.Operator,
                CreatedAt = ExampleData.CreatedAt,
                LastUpdated = ExampleData.CreatedAt
            }
        };

        //Count is the total number of records, not the page size.
        return new PagedList<UserGetPaginatedResponse>(items, 42);
    }
}
