using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orion.Api.AutoMapper.Output;
using Orion.Api.Controllers;
using Orion.Entities.Domain;
using Orion.Entities.Filter;
using Orion.Entities.ValueObjects.Pagination;
using Orion.Domain.Interfaces;
using Orion.Test.Controllers.BaseController;
using Orion.Test.MotherObjects;
using Xunit;

namespace Orion.Test.Controllers
{
    public class UsersControllerTestTest : BaseControllerTest
    {
        private UsersController _usersController;

        public UsersControllerTestTest()
        {
            SetupServiceMock();
        }

        [Fact]
        public async Task GetUsertById_WithValidId_ReturnsValidCustomer()
        {
            //arrange & act
            var result = await _usersController.Get(UserMotherObject.ValidAdminUser().PublicId);

            var contentResult = (OkObjectResult) result;
            var user = (UserOutput) contentResult.Value;

            //assert
            Assert.IsType<OkObjectResult>(contentResult);
            Assert.Equal(200, contentResult.StatusCode);

            Assert.IsType<UserOutput>(contentResult.Value);
            Assert.Equal(UserMotherObject.ValidAdminUser().Email, user.Email);
            Assert.Equal(UserMotherObject.ValidAdminUser().Name, user.Name);
        }

        [Fact]
        public async Task PostUser_WithValidData_CreateAUser()
        {
            //arrange & act
            var result = await _usersController.Post(UserMotherObject.ValidAdminUserInput());

            var contentResult = (CreatedResult) result;

            //assert
            Assert.IsType<CreatedResult>(contentResult);
            Assert.Equal(201, contentResult.StatusCode);
        }


        [Fact]
        public async Task PutUser_WithValidData_UpdateUser()
        {
            //arrange & act
            var result = await _usersController.Put(UserMotherObject.ValidAdminUser().PublicId, UserMotherObject.ValidAdminUserInput());

            var contentResult = (AcceptedResult) result;

            //assert
            Assert.IsType<AcceptedResult>(contentResult);
            Assert.Equal(202, contentResult.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_WithExistantId_DeleteUser()
        {
            //arrange & act
            var result = await _usersController.Delete(UserMotherObject.ValidAdminUser().PublicId);

            var contentResult = (NoContentResult)result;

            //assert
            Assert.IsType<NoContentResult>(contentResult);
            Assert.Equal(204, contentResult.StatusCode);
        }

        [Fact]
        public async Task GetUsers_WithValidFilter_ReturnsAListOfUsers()
        {
            //arrange & act
            var result = await _usersController.Get(new UserFilter());

            var contentResult = (OkObjectResult) result;
            var userPagedList = (PagedList<UserOutput>) contentResult.Value;

            //assert
            Assert.Equal(4, userPagedList.Count);
            Assert.Equal(200, contentResult.StatusCode);
        }

        private void SetupServiceMock()
        {
            var userServiceMock = new Mock<IUserService>();
            var userList = new List<User>
            {
                UserMotherObject.ValidAdminUser(),
                UserMotherObject.ValidAdminUser(),
                UserMotherObject.ValidAdminUser(),
                UserMotherObject.ValidAdminUser()
            };

            var userListPaginated = new PagedList<User>(userList, 4);

            userServiceMock.Setup(x => x.FindByIdAsync(UserMotherObject.ValidAdminUser().PublicId)).ReturnsAsync(UserMotherObject.ValidAdminUser());
            userServiceMock.Setup(x => x.AddAsync(It.IsAny<User>())).ReturnsAsync(UserMotherObject.ValidAdminUser());
            userServiceMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).Verifiable();
            userServiceMock.Setup(x => x.DeleteAsync(UserMotherObject.ValidAdminUser().PublicId)).Verifiable();
            userServiceMock.Setup(x => x.ListPaginateAsync(It.IsAny<UserFilter>())).
                ReturnsAsync(userListPaginated);

            _usersController = new UsersController(userServiceMock.Object, Mapper);
        }
    }
}
