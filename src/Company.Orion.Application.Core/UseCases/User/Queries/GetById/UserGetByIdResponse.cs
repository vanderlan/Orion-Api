using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Entities.Enuns;

namespace Company.Orion.Application.Core.UseCases.User.Queries.GetById;

public class UserGetByIdResponse
{
    public string PublicId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public UserProfile Profile { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime CreatedAt { get; set; }

    public static explicit operator UserGetByIdResponse(User user)
    {
        if (user is null)
            return default;

        return new UserGetByIdResponse
        {
            PublicId = user.PublicId,
            Name = user.Name,
            Email = user.Email,
            LastUpdated = user.LastUpdated,
            CreatedAt = user.CreatedAt,
            Profile = user.Profile
        };
    }

}