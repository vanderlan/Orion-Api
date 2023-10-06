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
        public async Task AddAsync_WithValidData_AddUserAsSuccess()
        {
            //arrange
            using var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();

            //act
            var userSaved = await userService.AddAsync(UserMotherObject.ValidAdminUser());
            
            var userFound = await userService.FindByIdAsync(userSaved.PublicId);
            
            //assert
            Assert.NotNull(userFound);
            Assert.Equal(UserMotherObject.ValidAdminUser().Password.ToSha512(), userFound.Password);
            Assert.Equal(userFound.Name, UserMotherObject.ValidAdminUser().Name);
            Assert.True(userFound.UserId > 0);

            await userService.DeleteAsync(userFound.PublicId);
        }

        [Fact]
        public async Task AddAsync_DuplicatedUser_ThrowsConflictException()
        {
            //arrange
            using var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();

            var userSaved = await userService.AddAsync(UserMotherObject.ValidAdminUser());

            //act && assert
            await Assert.ThrowsAsync<ConflictException>(() => userService.AddAsync(UserMotherObject.ValidAdminUser()));

            await userService.DeleteAsync(userSaved.PublicId);
        }

        [Fact]
        public async Task ListPaginateAsync_WithFilterByName_GetAllMatchedUsers()
        {
            //arrange
            var userCount = 1;

            using var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();

            var userSaved = await userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await userService.FindByIdAsync(userSaved.PublicId);

            //act
            var userPaginated = await userService.ListPaginateAsync(
                new UserFilter {
                    Query = UserMotherObject.ValidAdminUser().Name,
                    Entity = new User 
                    { 
                        Name = UserMotherObject.ValidAdminUser().Name 
                    } 
                });

            //assert
            Assert.Equal(userCount, userPaginated.Count);
            Assert.Contains(userPaginated.Items, x => x.Name == UserMotherObject.ValidAdminUser().Name);

            await userService.DeleteAsync(userFound.PublicId);
        }

        [Fact]
        public async Task AddAsync_WithInvalidData_ThrowsBusinessException()
        {
            //arrange
            using var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();

            //act & assert
            await Assert.ThrowsAsync<BusinessException>(() => userService.AddAsync(UserMotherObject.InvalidAdminUserWihoutPassword()));
        }

        [Fact]
        public async Task DeleteAsync_WithExistantId_RemoveUserAsSuccess()
        {
            //arrange
            using var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();

            var userSaved = await userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await userService.FindByIdAsync(userSaved.PublicId);

            Assert.NotNull(userFound);

            //act
            await userService.DeleteAsync(userFound.PublicId);

            var userDeleted = await userService.FindByIdAsync(userSaved.PublicId);

            //assert
            Assert.Null(userDeleted);
        }

        [Fact]
        public async Task UpdateAsync_WithValidData_UpdateUserAsSuccess()
        {
            //arrange
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

            //act
            var userEdited = await userService.FindByIdAsync(userSaved.PublicId);

            //assert
            Assert.Equal(userFound.Email, userEdited.Email);
            Assert.Equal(userFound.Password.ToSha512(), userEdited.Password);
            Assert.Equal(userFound.Name, userEdited.Name);

            await userService.DeleteAsync(userFound.PublicId);
        }

        #endregion

        #region User Authentication tests
        [Fact]
        public async Task LoginAsync_WithValidCredentials_LoginAsSuccess()
        {
            //arrange
            using var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();

            var userAdded = await userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await userService.FindByIdAsync(userAdded.PublicId);

            Assert.NotNull(userFound);

            //act
            var userLoged = await userService.LoginAsync(userFound.Email, UserMotherObject.ValidAdminUser().Password);

            //assert
            Assert.NotNull(userLoged);
            Assert.Equal(userLoged.Email, userAdded.Email);
            Assert.Equal(userLoged.Password, userAdded.Password);
            Assert.Equal(userLoged.Name, userAdded.Name);

            await userService.DeleteAsync(userFound.PublicId);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidCredentials_ThrowsUnauthorizedUserException()
        {
            //arrange
            using var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();

            var userAdded = await userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await userService.FindByIdAsync(userAdded.PublicId);

            //act & assert
            await Assert.ThrowsAsync<UnauthorizedUserException>(() => userService.LoginAsync(userFound.Email, "wrong pass"));
            Assert.NotNull(userFound);

            await userService.DeleteAsync(userFound.PublicId);
        }

        [Fact]
        public async Task RefreshTokenAddAync_WithValidEmail_AddARefreskToken()
        {
            //arrange
            using var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();

            var userAdded = await userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await userService.FindByIdAsync(userAdded.PublicId);

            Assert.NotNull(userFound);

            var refreshToken = Guid.NewGuid().ToString();

            var refreshTokenAdded = await userService.AddRefreshTokenAsync(new RefreshToken { Email = UserMotherObject.ValidAdminUser().Email, Refreshtoken = refreshToken });

            //act
            var userByRefreshToken = await userService.GetUserByRefreshTokenAsync(refreshTokenAdded.Refreshtoken);

            //assert
            Assert.NotNull(userByRefreshToken);
            Assert.Equal(userByRefreshToken.Email, userAdded.Email);
            Assert.Equal(userByRefreshToken.Password, userAdded.Password);
            Assert.Equal(userByRefreshToken.Name, userAdded.Name);

            await userService.DeleteAsync(userFound.PublicId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("Invalid refresh token")]
        public async Task GetUserByRefreshTokenAsync_WithInvalidId_ThrowsUnauthorizedUserException(string refreshTokenId)
        {
            //arrange
            using var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();
            var messages = scope.ServiceProvider.GetService<IStringLocalizer<OrionResources>>();

            var userAdded = await userService.AddAsync(UserMotherObject.ValidAdminUser());
            var userFound = await userService.FindByIdAsync(userAdded.PublicId);

            Assert.NotNull(userFound);

            var refreshToken = Guid.NewGuid().ToString();

            await userService.AddRefreshTokenAsync(new RefreshToken { Email = UserMotherObject.ValidAdminUser().Email, Refreshtoken = refreshToken });

            //act
            var exeption = await Assert.ThrowsAsync<UnauthorizedUserException>(() => userService.GetUserByRefreshTokenAsync(refreshTokenId));

            //assert
            Assert.Equal(exeption.Message, messages[UserMessages.InvalidRefreshToken]);

            await userService.DeleteAsync(userFound.PublicId);
        }

        [Fact]
        public void CryptoSha512Test()
        {
            const string passwordTest = "userPawssTest1234A%@&!";
            const string expectedHash = "8c890b40034e242c05f27eec302a1f552be2a0a879b25b546c38d73c096d04aa8dfbf013a6c7e63a06ef42a346035c0e2256726d5aecb628df7bf6b42804802a";

            Assert.Equal(expectedHash, passwordTest.ToSha512());
        }

        #endregion
    }
}
