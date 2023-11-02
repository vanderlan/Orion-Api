using Orion.Domain.Core.Entities.Enuns;

namespace Orion.Application.Core.Commands.LoginWithCredentials;

public class LoginWithCredentialsResponse
{
    public string PublicId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public UserProfile Profile { get; set; }
    public string RefreshToken { get; set; }
}