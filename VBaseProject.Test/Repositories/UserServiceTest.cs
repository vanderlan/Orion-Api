using System;
using System.Threading.Tasks;
using VBaseProject.Data.UnitOfWork;
using VBaseProject.Entities.Domain;
using VBaseProject.Service.Implementation;
using VBaseProject.Service.Interfaces;
using Xunit;

namespace VBaseProject.Test.Repositories
{
	public class UserServiceTest
    {
		private readonly IUnitOfWorkEntity _unitOfWork;
		private readonly IUserService _userService;

		public UserServiceTest()
		{
			_unitOfWork = new UnitOfWorkEntity(TestBootstrapper.GetInMemoryDbContextOptions());
			_userService = new UserService(_unitOfWork);
		}

		[Fact]
        public async Task AddValidUserTest()
        {
			var name = Guid.NewGuid().ToString();

			var testUser = new User {
				FirstName = name,
				Password = "123"
			};

			var userSaved = await _userService.AddAsync(testUser);

			var userFound = await _userService.FindByIdAsync(userSaved.PublicId);

			Assert.Equal(userFound.FirstName, name);
		}
    }
}
