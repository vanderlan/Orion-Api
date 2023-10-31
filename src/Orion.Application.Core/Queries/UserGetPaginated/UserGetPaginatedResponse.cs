using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Entities.Enuns;

namespace Orion.Application.Core.Queries.UserGetPaginated;

public class UserGetPaginatedResponse
{
    public string PublicId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public UserProfile Profile { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public static implicit operator UserGetPaginatedResponse(User user)
    {
        if (user is null)
            return default;

        return new UserGetPaginatedResponse
        {
            PublicId = user.PublicId,
            Name = user.Name,
            Email = user.Email,
            Profile = user.Profile,
            LastUpdated = user.LastUpdated,
            CreatedAt = user.CreatedAt
        };
    }
}