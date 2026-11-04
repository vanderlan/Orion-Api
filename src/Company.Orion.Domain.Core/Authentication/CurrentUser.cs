using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Company.Orion.Domain.Core.Authentication;

public class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

    public string Name => GetClaim(ClaimTypes.GivenName);
    public string Id => GetClaim(ClaimTypes.Sid);
    public string Email => GetClaim(ClaimTypes.Email);
    public string Profile => GetClaim(ClaimTypes.Role);

    public bool IsAuthenticated()
    {
        return Principal?.Identity?.IsAuthenticated is true;
    }

    public override string ToString()
    {
        return IsAuthenticated() ? $"Name: {Name} - Id: {Id} - Email: {Email} - Profile: {Profile}" : "Anonymous User";
    }

    private string GetClaim(string claimType)
    {
        return IsAuthenticated() ? Principal?.FindFirst(claimType)?.Value ?? string.Empty : string.Empty;
    }
}
