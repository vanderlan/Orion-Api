using System.ComponentModel;

namespace Orion.Domain.Entities.Enuns
{
    public enum UserProfile
    {
        [Description("Admin")]
        Admin = 1,

        [Description("Customer")]
        Customer = 2
    }
}
