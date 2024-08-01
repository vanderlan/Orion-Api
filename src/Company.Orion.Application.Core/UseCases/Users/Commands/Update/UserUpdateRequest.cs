using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Entities.Enuns;
using MediatR;

namespace Company.Orion.Application.Core.UseCases.Users.Commands.Update;

public class UserUpdateRequest : IRequest
{
    public string PublicId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public UserProfile Profile { get; set; }

    public static implicit operator User(UserUpdateRequest request)
    {
        if (request is null)
            return default;

        return new User
        {
            PublicId = request.PublicId,
            Name = request.Name,
            Email = request.Email,
            Profile = request.Profile
        };
    }
}
