
using Orion.Core.Domain.Entities.Enuns;
using Orion.Core.Domain.Authentication;

namespace Orion.Api.AutoMapper.Output;

public class UserOutput
{
    public string PublicId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public UserProfile Profile { get; set; }
    public string ProfileDescription => Profile == UserProfile.Admin ? AuthorizationConfiguration.Roles.Admin : AuthorizationConfiguration.Roles.Customer;
}
