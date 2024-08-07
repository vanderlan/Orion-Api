using Microsoft.AspNetCore.Authorization;

namespace Company.Orion.Api.Attributes;

public class AuthorizeForAttribute : AuthorizeAttribute
{
    public AuthorizeForAttribute(params string[] roles)
    {
        Roles = string.Join(",", roles);
    }
}
