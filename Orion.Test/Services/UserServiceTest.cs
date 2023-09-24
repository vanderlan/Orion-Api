using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;
using Orion.Entities.Domain;
using Orion.Entities.Filter;
using Orion.Resources;
using Orion.Domain.Exceptions;
using Orion.Domain.Extensions;
using Orion.Domain.Interfaces;
using Orion.Test.Configuration;
using Orion.Test.MotherObjects;
using Orion.Test.Services.BaseService;
using Xunit;
using static Orion.Resources.Messages.MessagesKeys;

namespace Orion.Test.Services
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
            using var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();

            var userSaved = await userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await userService.FindByIdAsync(userSaved.PublicId);
            
            Assert.NotNull(userFound);
            Assert.Equal(UserMotherObject.ValidAdminUser().Password.ToSha512(), userFound.Password);
            Assert.Equal(userFound.Name, UserMotherObject.ValidAdminUser().Name);
            Assert.True(userFound.UserId > 0);

            await userService.DeleteAsync(userFound.PublicId);
        }

        [Fact]
        public async Task AddDuplicatedUserTest()
        {
            using var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();

            var userSaved = await userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await userService.FindByIdAsync(userSaved.PublicId);

            Assert.NotNull(userFound);
            Assert.Equal(UserMotherObject.ValidAdminUser().Password.ToSha512(), userFound.Password);
            Assert.Equal(userFound.Name, UserMotherObject.ValidAdminUser().Name);

            await Assert.ThrowsAsync<ConflictException>(() => userService.AddAsync(UserMotherObject.ValidAdminUser()));

            await userService.DeleteAsync(userFound.PublicId);
        }
        [Fact]
        public async Task ListUserPaginatedFilterTest()
        {
            var userCount = 1;

            using var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();

            var userSaved = await userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await userService.FindByIdAsync(userSaved.PublicId);

            var userPaginated = await userService.ListPaginate(
                new UserFilter {
                    Query = UserMotherObject.ValidAdminUser().Name,
                    Entity = new User 
                    { 
                        Name = UserMotherObject.ValidAdminUser().Name 
                    } 
                });

            Assert.Equal(userCount, userPaginated.Count);
            Assert.Contains(userPaginated.Items, x => x.Name == UserMotherObject.ValidAdminUser().Name);

            await userService.DeleteAsync(userFound.PublicId);
        }

        [Fact]
        public async Task AddInvalidUserTest()
        {
            using var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();

            await Assert.ThrowsAsync<BusinessException>(() => userService.AddAsync(UserMotherObject.InvalidAdminUserWihoutPassword()));
        }

        [Fact]
        public async Task RemoveUserTest()
        {
            using var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();

            var userSaved = await userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await userService.FindByIdAsync(userSaved.PublicId);

            Assert.NotNull(userFound);

            await userService.DeleteAsync(userFound.PublicId);

            var userDeleted = await userService.FindByIdAsync(userSaved.PublicId);

            Assert.Null(userDeleted);
        }

        [Fact]
        public async Task EditUserTest()
        {
            using var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();

            var userSaved = await userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await userService.FindByIdAsync(userSaved.PublicId);

            Assert.NotNull(userFound);

            userFound.Name = "Jane";
            userFound.Email = "newemail@gmail.com";
            userFound.Password = "123";

            await userService.UpdateAsync(userFound);
            await userService.FindByIdAsync(userSaved.PublicId);

            var userEdited = await userService.FindByIdAsync(userSaved.PublicId);

            Assert.Equal(userFound.Email, userEdited.Email);
            Assert.Equal(userFound.Password, userEdited.Password);
            Assert.Equal(userFound.Name, userEdited.Name);

            await userService.DeleteAsync(userFound.PublicId);
        }

        #endregion

        #region User Authentication tests
        [Fact]
        public async Task LoginValidTest()
        {
            using var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();

            var userAdded = await userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await userService.FindByIdAsync(userAdded.PublicId);

            Assert.NotNull(userFound);

            var userLoged = await userService.LoginAsync(userFound.Email, UserMotherObject.ValidAdminUser().Password);

            Assert.NotNull(userLoged);

            Assert.Equal(userLoged.Email, userAdded.Email);
            Assert.Equal(userLoged.Password, userAdded.Password);
            Assert.Equal(userLoged.Name, userAdded.Name);

            await userService.DeleteAsync(userFound.PublicId);
        }

        [Fact]
        public async Task LoginInvalidPassTest()
        {
            using var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();

            var userAdded = await userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await userService.FindByIdAsync(userAdded.PublicId);

            Assert.NotNull(userFound);

            await Assert.ThrowsAsync<UnauthorizedUserException>(() => userService.LoginAsync(userFound.Email, "wrong pass"));

            await userService.DeleteAsync(userFound.PublicId);
        }

        [Fact]
        public async Task AddRefreshTokenValidTest()
        {
            using var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();

            var userAdded = await userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await userService.FindByIdAsync(userAdded.PublicId);

            Assert.NotNull(userFound);

            var refreshToken = Guid.NewGuid().ToString();

            var refreshTokenAdded = await userService.AddRefreshToken(new RefreshToken { Email = UserMotherObject.ValidAdminUser().Email, Refreshtoken = refreshToken });

            var userByRefreshToken = await userService.GetUserByRefreshToken(refreshTokenAdded.Refreshtoken);

            Assert.NotNull(userByRefreshToken);

            Assert.Equal(userByRefreshToken.Email, userAdded.Email);
            Assert.Equal(userByRefreshToken.Password, userAdded.Password);
            Assert.Equal(userByRefreshToken.Name, userAdded.Name);

            await userService.DeleteAsync(userFound.PublicId);
        }

        [Fact]
        public async Task RefreshTokenNullTest()
        {
            using var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();
            var messages = scope.ServiceProvider.GetService<IStringLocalizer<OrionResources>>();

            var userAdded = await userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await userService.FindByIdAsync(userAdded.PublicId);

            Assert.NotNull(userFound);

            var refreshToken = Guid.NewGuid().ToString();

            await userService.AddRefreshToken(new RefreshToken { Email = UserMotherObject.ValidAdminUser().Email, Refreshtoken = refreshToken });

            var exeption = await Assert.ThrowsAsync<UnauthorizedUserException>(() => userService.GetUserByRefreshToken(null));

            Assert.Equal(exeption.Message, messages[UserMessages.InvalidRefreshToken]);

            await userService.DeleteAsync(userFound.PublicId);
        }

        [Fact]
        public async Task RefreshTokenInvalidTest()
        {
            using var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();
            var messages = scope.ServiceProvider.GetService<IStringLocalizer<OrionResources>>();

            var userAdded = await userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await userService.FindByIdAsync(userAdded.PublicId);

            Assert.NotNull(userFound);

            var refreshToken = Guid.NewGuid().ToString();

            await userService.AddRefreshToken(new RefreshToken { Email = UserMotherObject.ValidAdminUser().Email, Refreshtoken = refreshToken });

            var exeption = await Assert.ThrowsAsync<UnauthorizedUserException>(() => userService.GetUserByRefreshToken("wrong refresh token"));

            Assert.Equal(exeption.Message, messages[UserMessages.InvalidRefreshToken]);

            await userService.DeleteAsync(userFound.PublicId);
        }

        [Fact]
        public void CryptoSha512Test()
        {
            var stringValidTest = "userPawssTest1234A%@&!";
            var expectedResult = "8c890b40034e242c05f27eec302a1f552be2a0a879b25b546c38d73c096d04aa8dfbf013a6c7e63a06ef42a346035c0e2256726d5aecb628df7bf6b42804802a";

            Assert.Equal(expectedResult, stringValidTest.ToSha512());
        }

        #endregion
    }
}
