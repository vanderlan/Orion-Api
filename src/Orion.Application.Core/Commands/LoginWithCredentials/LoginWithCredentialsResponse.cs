using Orion.Domain.Core.Authentication;
using Orion.Domain.Core.Entities.Enuns;

namespace Orion.Application.Core.Commands.LoginWithCredentials;

public class LoginWithCredentialsResponse
{
    public string PublicId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public UserProfile Profile { get; set; }
    public string ProfileDescription => Profile == UserProfile.Admin ? AuthorizationConfiguration.Roles.Admin : AuthorizationConfiguration.Roles.Customer;
    public string RefreshToken { get; set; }
}