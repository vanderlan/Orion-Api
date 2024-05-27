using System.ComponentModel;
using Company.Orion.Domain.Core.Authentication;
using static Company.Orion.Domain.Core.Authentication.AuthorizationConfiguration;

namespace Company.Orion.Domain.Core.Entities.Enuns;

public enum UserProfile
{
    [Description(AuthorizationConfiguration.Roles.Admin)]
    Admin = 1,

    [Description(AuthorizationConfiguration.Roles.Operator)]
    Operator = 2
}
