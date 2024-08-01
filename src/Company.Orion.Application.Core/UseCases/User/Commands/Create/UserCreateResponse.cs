using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Entities.Enuns;

namespace Company.Orion.Application.Core.UseCases.User.Commands.Create;

public class UserCreateResponse
{
    public string PublicId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public UserProfile Profile { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime CreatedAt { get; set; }

    public static explicit operator UserCreateResponse(User user)
    {
        if (user is null)
            return default;

        return new UserCreateResponse
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