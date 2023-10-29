using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Orion.Domain.Core.Services.Interfaces;
using Xunit;
using Orion.Test.Api.Controllers.BaseController;
using Orion.Domain.Core.Entities;
using Orion.Api.Controllers.V1;
using Orion.Application.Core.Queries.UserGetById;
using Orion.Application.Core.Queries.UserGetPaginated;
using Orion.Domain.Core.Filters;
using Orion.Domain.Core.ValueObjects.Pagination;
using Orion.Test.Faker;

namespace Orion.Test.Api.Controllers;

public class UsersControllerTestTest : BaseControllerTest
{
    private UsersController _usersController;
    private readonly User _validUser = UserFaker.Get();

    public UsersControllerTestTest()
    {
        SetupServiceMock();
    }

    [Fact]
    public async Task GetUsertById_WithValidId_ReturnsValidUser()
    {
        //arrange & act
        var result = await _usersController.Get(_validUser.PublicId);

        var contentResult = (OkObjectResult)result;
        var user = (UserGetByIdResponse)contentResult.Value;

        //assert
        Assert.IsType<OkObjectResult>(contentResult);
        Assert.Equal(200, contentResult.StatusCode);

        Assert.IsType<UserGetByIdResponse>(contentResult.Value);
        Assert.Equal(_validUser.Email, user.Email);
        Assert.Equal(_validUser.Name, user.Name);
    }

    [Fact]
    public async Task PostUser_WithValidData_CreateAUser()
    {
        //arrange & act
        var result = await _usersController.Post(UserFaker.GetUserCreateRequest());

        var contentResult = (CreatedResult)result;

        //assert
        Assert.IsType<CreatedResult>(contentResult);
        Assert.Equal(201, contentResult.StatusCode);
    }


    [Fact]
    public async Task PutUser_WithValidData_UpdateUser()
    {
        //arrange & act
        var result = await _usersController.Put(_validUser.PublicId, UserFaker.GetUserUpdateRequest());

        var contentResult = (AcceptedResult)result;

        //assert
        Assert.IsType<AcceptedResult>(contentResult);
        Assert.Equal(202, contentResult.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_WithExistantId_DeleteUser()
    {
        //arrange & act
        var result = await _usersController.Delete(_validUser.PublicId);

        var contentResult = (NoContentResult)result;

        //assert
        Assert.IsType<NoContentResult>(contentResult);
        Assert.Equal(204, contentResult.StatusCode);
    }

    [Fact]
    public async Task GetUsers_WithValidFilter_ReturnsAListOfUsers()
    {
        //arrange & act
        var result = await _usersController.Get(new UserGetPaginatedRequest());

        var contentResult = (OkObjectResult)result;
        var userPagedList = (PagedList<User>)contentResult.Value;

        //assert
        Assert.Equal(4, userPagedList.Count);
        Assert.Equal(200, contentResult.StatusCode);
    }

    private void SetupServiceMock()
    {
        var mediatorMock = new Mock<IMediator>();
        var userList = new List<User>(4)
        {
            _validUser,
            UserFaker.Get(),
            UserFaker.Get(),
            UserFaker.Get()
        };

        var userListPaginated = new PagedList<User>(userList, 4);

        // mediatorMock.Setup(x => x.FindByIdAsync(_validUser.PublicId)).ReturnsAsync(_validUser);
        // mediatorMock.Setup(x => x.AddAsync(It.IsAny<User>())).ReturnsAsync(UserFaker.Get());
        // mediatorMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).Verifiable();
        // mediatorMock.Setup(x => x.DeleteAsync(_validUser.PublicId)).Verifiable();
        // mediatorMock.Setup(x => x.ListPaginateAsync(It.IsAny<UserFilter>())).
        //     ReturnsAsync(userListPaginated);

        _usersController = new UsersController(mediatorMock.Object);
    }
}
