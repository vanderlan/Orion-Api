using System.Threading.Tasks;
using VBaseProject.Data.UnitOfWork;
using VBaseProject.Service.Exceptions;
using VBaseProject.Service.Extensions;
using VBaseProject.Service.Implementation;
using VBaseProject.Service.Interfaces;
using VBaseProject.Test.Configuration;
using VBaseProject.Test.MotherObjects;
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
			_userService = new UserService(_unitOfWork, null);
		}

		[Fact]
        public async Task AddValidUserTest()
        {
			var userSaved = await _userService.AddAsync(UserMotherObject.ValidAdminUser());
			var userFound = await _userService.FindByIdAsync(userSaved.PublicId);

			Assert.NotNull(userFound);
			Assert.Equal(UserMotherObject.ValidAdminUser().Password.ToSHA512(), userFound.Password);
			Assert.Equal(userFound.FirstName, UserMotherObject.ValidAdminUser().FirstName);
		}

		[Fact]
		public async Task AddInvalidUserTest()
		{
			await Assert.ThrowsAsync<BusinessException>(() => _userService.AddAsync(UserMotherObject.InvalidAdminUserWihoutPassword()));
		}

		[Fact]
		public async Task RemoveUserTest()
		{
			var userSaved = await _userService.AddAsync(UserMotherObject.ValidAdminUser());
			var userFound = await _userService.FindByIdAsync(userSaved.PublicId);

			Assert.NotNull(userFound);

			await _userService.DeleteAsync(userFound.PublicId);

			var userDeleted = await _userService.FindByIdAsync(userSaved.PublicId);

			Assert.Null(userDeleted);
		}

		[Fact]
		public async Task EditUserTest()
		{
			var userSaved = await _userService.AddAsync(UserMotherObject.ValidAdminUser());
			var userFound = await _userService.FindByIdAsync(userSaved.PublicId);

			Assert.NotNull(userFound);

			userFound.FirstName = "Jane";
			userFound.LastName = "White";
			userFound.Email = "newemail@gmail.com";
			userFound.Password = "123";

			await _userService.UpdateAsync(userFound);
			await _userService.FindByIdAsync(userSaved.PublicId);

			var userEdited = await _userService.FindByIdAsync(userSaved.PublicId);
			
			Assert.Equal(userFound.Email, userEdited.Email);
			Assert.Equal(userFound.Password, userEdited.Password);
			Assert.Equal(userFound.LastName, userEdited.LastName);
			Assert.Equal(userFound.FirstName, userEdited.FirstName);
		}
	}
}
