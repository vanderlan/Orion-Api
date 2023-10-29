using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Orion.Api.AutoMapper.Output;
using Orion.Api.Configuration;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Exceptions;
using Orion.Domain.Core.Extensions;
using Orion.Domain.Core.Services.Interfaces;
using Orion.Croscutting.Resources;
using Orion.Test.Configuration;
using Orion.Test.Domain.Services.BaseService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orion.Domain.Core.Filters;
using Orion.Test.Faker;
using Xunit;
using static Orion.Croscutting.Resources.Messages.MessagesKeys;

namespace Orion.Test.Domain.Services;

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

        var user = UserFaker.Get();

        //act
        var userSaved = await userService.AddAsync(user);

        var userFound = await userService.FindByIdAsync(userSaved.PublicId);

        //assert
        Assert.NotNull(userFound);
        Assert.Equal(user.Password, userFound.Password);
        Assert.Equal(userFound.Name, user.Name);
        Assert.True(userFound.UserId > 0);

        await userService.DeleteAsync(userFound.PublicId);
    }

    [Fact]
    public async Task AddAsync_DuplicatedUser_ThrowsConflictException()
    {
        //arrange
        using var scope = ServiceProvider.CreateScope();
        var userService = scope.ServiceProvider.GetService<IUserService>();

        var user = UserFaker.Get();
        var user2 = UserFaker.Get();
        user2.Email = user.Email;

        var userSaved = await userService.AddAsync(user);

        //act && assert
        await Assert.ThrowsAsync<ConflictException>(() => userService.AddAsync(user2));
    }

    [Fact]
    public async Task ListPaginateAsync_WithFilterByName_GetAllMatchedUsers()
    {
        //arrange
        var userCount = 1;

        using var scope = ServiceProvider.CreateScope();
        var userService = scope.ServiceProvider.GetService<IUserService>();

        var user = UserFaker.Get();

        var userSaved = await userService.AddAsync(user);
        var userFound = await userService.FindByIdAsync(userSaved.PublicId);

        //act
        var userPaginated = await userService.ListPaginateAsync(
            new BaseFilter<User>
            {
                Query = user.Name,
                Entity = new ()
                {
                    Name = user.Name
                }
            });

        //assert
        Assert.Equal(userCount, userPaginated.Count);
        Assert.Contains(userPaginated.Items, x => x.Name == user.Name);

        await userService.DeleteAsync(userFound.PublicId);
    }

    [Fact]
    public async Task AddAsync_WithInvalidData_ThrowsBusinessException()
    {
        //arrange
        using var scope = ServiceProvider.CreateScope();
        var userService = scope.ServiceProvider.GetService<IUserService>();

        var user = UserFaker.Get();
        user.Password = null;

        //act & assert
        await Assert.ThrowsAsync<BusinessException>(() => userService.AddAsync(user));
    }

    [Fact]
    public async Task DeleteAsync_WithExistantId_RemoveUserAsSuccess()
    {
        //arrange
        using var scope = ServiceProvider.CreateScope();
        var userService = scope.ServiceProvider.GetService<IUserService>();

        var userSaved = await userService.AddAsync(UserFaker.Get());
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

        var user = UserFaker.Get();
        user.Password = "123456789";

        var userSaved = await userService.AddAsync(user);

        userSaved.Name = "Jane";
        userSaved.Email = "newemail@gmail.com";
        userSaved.Password = "123";

        await userService.UpdateAsync(userSaved);

        //act
        var userEdited = await userService.FindByIdAsync(userSaved.PublicId);

        //assert
        Assert.Equal(userSaved.Email, userEdited.Email);
        Assert.Equal(userSaved.Password.ToSha512(), userEdited.Password);
        Assert.Equal(userSaved.Name, userEdited.Name);

        await userService.DeleteAsync(userSaved.PublicId);
    }

    #endregion

    #region User Authentication tests
    [Fact]
    public async Task LoginAsync_WithValidCredentials_LoginAsSuccess()
    {
        //arrange
        using var scope = ServiceProvider.CreateScope();
        var userService = scope.ServiceProvider.GetService<IUserService>();
        var userPassword = "123";

        var user = UserFaker.Get();
        user.Password = userPassword;

        var userAdded = await userService.AddAsync(user);
        var userFound = await userService.FindByIdAsync(userAdded.PublicId);

        Assert.NotNull(userFound);

        //act
        var userLoged = await userService.LoginAsync(userFound.Email, userPassword);

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

        var userAdded = await userService.AddAsync(UserFaker.Get());
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

        var userAdded = await userService.AddAsync(UserFaker.Get());
        var userFound = await userService.FindByIdAsync(userAdded.PublicId);
        Assert.NotNull(userFound);

        var refreshToken = Guid.NewGuid().ToString();

        var (Token, _) = AuthenticationConfiguration.CreateToken(new UserOutput { Email = userAdded.Email, Name = userAdded.Name, PublicId = userAdded.PublicId }, GetCofiguration());

        var refreshTokenAdded = await userService.AddRefreshTokenAsync(new RefreshToken { Email = userAdded.Email, Refreshtoken = refreshToken });

        //act
        var userByRefreshToken = await userService.SignInWithRehreshTokenAsync(refreshTokenAdded.Refreshtoken, Token);

        //assert
        Assert.NotNull(userByRefreshToken);
        Assert.Equal(userByRefreshToken.Email, userAdded.Email);
        Assert.Equal(userByRefreshToken.Password, userAdded.Password);
        Assert.Equal(userByRefreshToken.Name, userAdded.Name);

        await userService.DeleteAsync(userFound.PublicId);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("Invalid refresh token", "invalid old token")]
    public async Task GetUserByRefreshTokenAsync_WithInvalidId_ThrowsUnauthorizedUserException(string refreshToken, string token)
    {
        //arrange
        using var scope = ServiceProvider.CreateScope();
        var userService = scope.ServiceProvider.GetService<IUserService>();
        var messages = scope.ServiceProvider.GetService<IStringLocalizer<OrionResources>>();

        var userAdded = await userService.AddAsync(UserFaker.Get());
        var userFound = await userService.FindByIdAsync(userAdded.PublicId);

        Assert.NotNull(userFound);

        await userService.AddRefreshTokenAsync(new RefreshToken { Email = userAdded.Email, Refreshtoken = Guid.NewGuid().ToString() });

        //act
        var exeption = await Assert.ThrowsAsync<UnauthorizedUserException>(() => userService.SignInWithRehreshTokenAsync(refreshToken, token));

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

    private static IConfiguration GetCofiguration()
    {

        var inMemorySettings = new Dictionary<string, string> {
            {"JwtOptions:SymmetricSecurityKey", "5cCI6IkpXVCJ9.eyJlbWFpbCI6InZhbmRlcmxhbi5nc0BnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJhZG1p"},
            {"JwtOptions:Issuer", "http://www.myapplication.com"},
            {"JwtOptions:Audience", "http://www.myapplication.com"},
            {"JwtOptions:TokenExpirationMinutes", "15"}
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    #endregion
}
