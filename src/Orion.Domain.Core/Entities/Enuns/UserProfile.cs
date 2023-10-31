using System.ComponentModel;
using static Orion.Domain.Core.Authentication.AuthorizationConfiguration;

namespace Orion.Domain.Core.Entities.Enuns;

public enum UserProfile
{
    [Description(Roles.Admin)]
    Admin = 1,

    [Description(Roles.Customer)]
    Customer = 2
}
