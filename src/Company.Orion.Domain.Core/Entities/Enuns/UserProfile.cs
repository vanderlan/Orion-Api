using System.ComponentModel;
using static Company.Orion.Domain.Core.Authentication.AuthorizationConfiguration;

namespace Company.Orion.Domain.Core.Entities.Enuns;

public enum UserProfile
{
    [Description(Roles.Admin)]
    Admin = 1,

    [Description(Roles.Operator)]
    Operator = 2
}
