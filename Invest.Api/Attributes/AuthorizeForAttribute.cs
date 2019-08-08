using Microsoft.AspNetCore.Authorization;

namespace Invest.Api.Attributes
{
    public class AuthorizeForAttribute : AuthorizeAttribute
    {
        public AuthorizeForAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }
}
