using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;
using VBaseProject.Entities.Domain;
using VBaseProject.Resources;
using VBaseProject.Service.Exceptions;
using VBaseProject.Service.Extensions;
using VBaseProject.Service.Interfaces;
using VBaseProject.Test.Configuration;
using VBaseProject.Test.MotherObjects;
using VBaseProject.Test.Services.BaseService;
using Xunit;
using static VBaseProject.Resources.Messages.MessagesKeys;

//coverlet .\bin\Debug\netcoreapp3.1\VBaseProject.Api.dll --target "dotnet" --targetargs "test --no-build"

namespace VBaseProject.Test.Services
{
    public class UserServiceTest : BaseServiceTest
    {
        public UserServiceTest(DependencyInjectionSetupFixture fixture) : base(fixture)
        {

        }

        #region User CRUD tests
        [Fact]
        public async Task AddValidUserTest()
        {
            using var scope = _serviceProvider.CreateScope();
            var _userService = scope.ServiceProvider.GetService<IUserService>();

            var userSaved = await _userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await _userService.FindByIdAsync(userSaved.PublicId);

            Assert.NotNull(userFound);
            Assert.Equal(UserMotherObject.ValidAdminUser().Password.ToSHA512(), userFound.Password);
            Assert.Equal(userFound.FirstName, UserMotherObject.ValidAdminUser().FirstName);
        }

        [Fact]
        public async Task AddInvalidUserTest()
        {
            using var scope = _serviceProvider.CreateScope();
            var _userService = scope.ServiceProvider.GetService<IUserService>();

            await Assert.ThrowsAsync<BusinessException>(() => _userService.AddAsync(UserMotherObject.InvalidAdminUserWihoutPassword()));
        }

        [Fact]
        public async Task RemoveUserTest()
        {
            using var scope = _serviceProvider.CreateScope();
            var _userService = scope.ServiceProvider.GetService<IUserService>();

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
            using var scope = _serviceProvider.CreateScope();
            var _userService = scope.ServiceProvider.GetService<IUserService>();

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

        #endregion

        #region User Authentication tests
        [Fact]
        public async Task LoginValidTest()
        {
            using var scope = _serviceProvider.CreateScope();
            var _userService = scope.ServiceProvider.GetService<IUserService>();

            var userAdded = await _userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await _userService.FindByIdAsync(userAdded.PublicId);

            Assert.NotNull(userFound);

            var userLoged = await _userService.LoginAsync(userFound.Email, UserMotherObject.ValidAdminUser().Password);

            Assert.NotNull(userLoged);

            Assert.Equal(userLoged.Email, userAdded.Email);
            Assert.Equal(userLoged.Password, userAdded.Password);
            Assert.Equal(userLoged.LastName, userAdded.LastName);
            Assert.Equal(userLoged.FirstName, userAdded.FirstName);
        }

        [Fact]
        public async Task LoginInvalidPassTest()
        {
            using var scope = _serviceProvider.CreateScope();
            var _userService = scope.ServiceProvider.GetService<IUserService>();

            var userAdded = await _userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await _userService.FindByIdAsync(userAdded.PublicId);

            Assert.NotNull(userFound);

            await Assert.ThrowsAsync<UnauthorizedUserException>(() => _userService.LoginAsync(userFound.Email, "wrong pass"));
        }

        [Fact]
        public async Task AddRefreshTokenValidTest()
        {
            using var scope = _serviceProvider.CreateScope();
            var _userService = scope.ServiceProvider.GetService<IUserService>();

            var userAdded = await _userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await _userService.FindByIdAsync(userAdded.PublicId);

            Assert.NotNull(userFound);

            var refreshToken = Guid.NewGuid().ToString();

            await _userService.AddRefreshToken(new RefreshToken { Email = UserMotherObject.ValidAdminUser().Email, Refreshtoken = refreshToken });

            var userByRefreshToken = await _userService.GetUserByRefreshToken(refreshToken);

            Assert.NotNull(userByRefreshToken);

            Assert.Equal(userByRefreshToken.Email, userAdded.Email);
            Assert.Equal(userByRefreshToken.Password, userAdded.Password);
            Assert.Equal(userByRefreshToken.LastName, userAdded.LastName);
            Assert.Equal(userByRefreshToken.FirstName, userAdded.FirstName);
        }

        [Fact]
        public async Task RefreshTokenNullTest()
        {
            using var scope = _serviceProvider.CreateScope();
            var _userService = scope.ServiceProvider.GetService<IUserService>();
            var messages = scope.ServiceProvider.GetService<IStringLocalizer<VBaseProjectResources>>();

            var userAdded = await _userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await _userService.FindByIdAsync(userAdded.PublicId);

            Assert.NotNull(userFound);

            var refreshToken = Guid.NewGuid().ToString();

            await _userService.AddRefreshToken(new RefreshToken { Email = UserMotherObject.ValidAdminUser().Email, Refreshtoken = refreshToken });

            var exeption = await Assert.ThrowsAsync<UnauthorizedUserException>(() => _userService.GetUserByRefreshToken(null));

            Assert.Equal(exeption.Message, messages[UserMessages.InvalidRefreshToken]);
        }

        [Fact]
        public async Task RefreshTokenInvalidTest()
        {
            using var scope = _serviceProvider.CreateScope();
            var _userService = scope.ServiceProvider.GetService<IUserService>();
            var messages = scope.ServiceProvider.GetService<IStringLocalizer<VBaseProjectResources>>();

            var userAdded = await _userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await _userService.FindByIdAsync(userAdded.PublicId);

            Assert.NotNull(userFound);

            var refreshToken = Guid.NewGuid().ToString();

            await _userService.AddRefreshToken(new RefreshToken { Email = UserMotherObject.ValidAdminUser().Email, Refreshtoken = refreshToken });

            var exeption = await Assert.ThrowsAsync<UnauthorizedUserException>(() => _userService.GetUserByRefreshToken("wrong refresh token"));

            Assert.Equal(exeption.Message, messages[UserMessages.InvalidRefreshToken]);
        }
        #endregion
    }
}
