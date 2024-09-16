using Microsoft.AspNetCore.Authorization;

namespace Company.Orion.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
public class AuthorizeForAttribute : AuthorizeAttribute
{
    public AuthorizeForAttribute(params string[] roles)
    {
        Roles = string.Join(",", roles);
    }
}
