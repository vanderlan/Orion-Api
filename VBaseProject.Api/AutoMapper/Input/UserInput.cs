using VBaseProject.Entities.Enuns;

namespace VBaseProject.Api.AutoMapper.Input
{
    public class UserInput
    {
        public string PublicId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserProfile Profile { get; set; }
    }
}
