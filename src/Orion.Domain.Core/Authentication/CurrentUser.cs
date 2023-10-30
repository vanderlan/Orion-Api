using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Orion.Domain.Core.Authentication;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor;
    private readonly List<Claim> _claims;

    public CurrentUser(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
        _claims = _accessor.HttpContext.User.Claims.ToList();
    }

    public string Name => IsAuthenticated() ? _claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value : string.Empty;
    public string Id => IsAuthenticated() ? _claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value: string.Empty;
    public string Email => IsAuthenticated() ? _claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value: string.Empty;
    public string Profile => IsAuthenticated() ? _claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value: string.Empty;

    public bool IsAuthenticated()
    {
        return _accessor.HttpContext.User.Identity.IsAuthenticated;
    }

    public override string ToString() 
    {
        if(IsAuthenticated())
            return $"Name: {Name} - Id: {Id} - Email: {Email} - Profile: {Profile}";

        return "Anonymous User";
    }
}
