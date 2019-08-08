using Invest.Entities.Enuns;
using Invest.Service.Authentication;

namespace Invest.Api.AutoMapper
{
    public class UserOutput
    {
        public string PublicId { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public UserProfile Profile { get; set; }
        public string ProfileDescription => Profile == UserProfile.Admin ? AuthenticationConfiguration.Roles.Admin : AuthenticationConfiguration.Roles.Customer;
        public string FirstName { get; set; }
    }
}
