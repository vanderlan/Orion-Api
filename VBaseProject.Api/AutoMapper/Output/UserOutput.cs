
using VBaseProject.Entities.Enuns;
using VBaseProject.Domain.Authentication;

namespace VBaseProject.Api.AutoMapper.Output
{
    public class UserOutput
    {
        public string PublicId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public UserProfile Profile { get; set; }
        public string ProfileDescription => Profile == UserProfile.Admin ? AuthorizationConfiguration.Roles.Admin : AuthorizationConfiguration.Roles.Customer;
    }
}
