namespace Orion.Api.Models;

public class UserApiTokenModel
{
    public string Token { get; init; }
    public string RefreshToken { get; init; }
    public DateTime Expiration { get; init; }
}
