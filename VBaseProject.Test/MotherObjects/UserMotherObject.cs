using VBaseProject.Api.AutoMapper.Input;
using VBaseProject.Api.AutoMapper.Output;
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
                Name = "Michael Philips",
                Email = "michaelfilips@gmail.com",
                Password = "123",
                Profile = UserProfile.Admin,
                PublicId = "aeb2d208-d3c6-4304-8d85-30a899f0043a"
            };
        }

        public static UserInput ValidAdminUserInput()
        {
            return new UserInput
            {
                Name = "Michael Philips",
                Email = "michaelfilips@gmail.com",
                Password = "123",
                Profile = UserProfile.Admin
            };
        }

        public static UserOutput ValidAdminUserOutput()
        {
            return new UserOutput
            {
                Name = "Michael Philips",
                Email = "michaelfilips@gmail.com",
                Profile = UserProfile.Admin
            };
        }

        public static User InvalidAdminUserWihoutPassword()
        {
            return new User
            {
                Name = "Michael Philips",
                Email = "michaelfilips@gmail.com",
                Password = null,
                Profile = UserProfile.Admin
            };
        }
    }
}
