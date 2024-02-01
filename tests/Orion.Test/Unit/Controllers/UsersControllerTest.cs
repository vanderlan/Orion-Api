using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Orion.Api.Controllers.V1;
using Orion.Application.Core.Queries.UserGetById;
using Orion.Application.Core.Queries.UserGetPaginated;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.ValueObjects.Pagination;
using Orion.Test.Unit.Controllers.BaseController;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Orion.Test.Shared.Faker;
using Xunit;

namespace Orion.Test.Unit.Controllers;

public class UsersControllerTestTest : BaseControllerTest
{
    private UsersController _usersController;
    private readonly User _validUser = UserFaker.Get();

    public UsersControllerTestTest()
    {
        SetupMediatorMock();
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
        var userPagedList = (PagedList<UserGetPaginatedResponse>)contentResult.Value;

        //assert
        Assert.Equal(4, userPagedList.Count);
        Assert.Equal(200, contentResult.StatusCode);
    }

    private void SetupMediatorMock()
    {
        var mediatorMock = new Mock<IMediator>();

        var userList = new List<UserGetPaginatedResponse>(4)
        {
            _validUser,
            UserFaker.Get(),
            UserFaker.Get(),
            UserFaker.Get()
        };

        var userListPaginated = new PagedList<UserGetPaginatedResponse>(userList, 4);

        mediatorMock.Setup(x => x.Send(It.IsAny<UserGetPaginatedRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userListPaginated);

        mediatorMock.Setup(x => x.Send(It.IsAny<UserGetByIdRequest>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync((UserGetByIdResponse)_validUser);

        _usersController = new UsersController(mediatorMock.Object);
    }
}
