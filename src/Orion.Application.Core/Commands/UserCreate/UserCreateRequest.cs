using MediatR;
using Orion.Domain.Core.Entities;

namespace Orion.Application.Core.Commands.UserCreate;

public class UserCreateRequest : IRequest <UserCreateResponse>
{
    public string Name { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }

    public static implicit operator User(UserCreateRequest request)
    {
        if (request is null)
            return default;
        
        return new User
        {
            Name = request.Name,
            Password = request.Password,
            Email = request.Email
        };
    }
}

