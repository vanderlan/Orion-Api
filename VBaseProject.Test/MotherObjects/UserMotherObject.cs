using VBaseProject.Entities.Domain;
using VBaseProject.Entities.Enuns;

namespace VBaseProject.Test.MotherObjects
{
    public class UserMotherObject
    {
        public static User ValidAdminUser()
        {
            return new User 
            {
                FirstName = "Michael",
                LastName = "Philips",
                Email = "michaelfilips@gmail.com",
                Password = "123",
                Profile = UserProfile.Admin
            };
        }

        public static User InvalidAdminUserWihoutPassword()
        {
            return new User
            {
                FirstName = "Michael",
                LastName = "Philips",
                Email = "michaelfilips@gmail.com",
                Password = null,
                Profile = UserProfile.Admin
            };
        }
    }
}
