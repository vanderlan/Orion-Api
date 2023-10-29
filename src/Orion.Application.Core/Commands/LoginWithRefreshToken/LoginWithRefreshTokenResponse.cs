using Orion.Application.Core.Commands.LoginWithCredentials;
using Orion.Domain.Core.Entities;

namespace Orion.Application.Core.Commands.LoginWithRefreshToken;

public class LoginWithRefreshTokenResponse : LoginWithCredentialsResponse
{
    public static explicit operator LoginWithRefreshTokenResponse(User user)
    {
        return new LoginWithRefreshTokenResponse
        {
            Email = user.Email,
            Name = user.Name,
            PublicId = user.PublicId,
            Profile = user.Profile
        };
    }
}