using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using VBaseProject.Api.AutoMapper.Output;
using VBaseProject.Api.Controllers;
using VBaseProject.Entities.Domain;
using VBaseProject.Entities.Filter;
using VBaseProject.Entities.ValueObjects.Pagination;
using VBaseProject.Service.Interfaces;
using VBaseProject.Test.Controllers.BaseController;
using VBaseProject.Test.MotherObjects;
using Xunit;

namespace VBaseProject.Test.Controllers
{
    public class CustomersControllerTestTest : BaseControllerTest
    {
        private CustomersController _customersController;

        public CustomersControllerTestTest()
        {
            SetupServiceMock();
        }

        [Fact]
        public async Task GetCustomerValidTest()
        {
            var result = await _customersController.Get(CustomerMotherObject.ValidCustomer().PublicId);

            var contentResult = (OkObjectResult) result;
            var customer = (CustomerOutput) contentResult.Value;

            Assert.IsType<OkObjectResult>(contentResult);
            Assert.Equal(200, contentResult.StatusCode);

            Assert.IsType<CustomerOutput>(contentResult.Value);
            Assert.Equal(CustomerMotherObject.ValidCustomer().Name, customer.Name);
            Assert.Equal(CustomerMotherObject.ValidCustomer().Address, customer.Address);
        }

        [Fact]
        public async Task CreateCustomerValidTest()
        {
            var result = await _customersController.Post(CustomerMotherObject.ValidCustomerInput());

            var contentResult = (CreatedResult) result;
            var contentResultObject = (CustomerOutput)contentResult.Value;

            Assert.IsType<CreatedResult>(contentResult);
            Assert.Equal(201, contentResult.StatusCode);
            Assert.NotNull(contentResult.Value);
            Assert.Equal(CustomerMotherObject.ValidCustomerInput().Name, contentResultObject.Name);
            Assert.Equal(CustomerMotherObject.ValidCustomerInput().Address, contentResultObject.Address);
        }

        [Fact]
        public async Task EditCustomerValidTest()
        {
            var result = await _customersController.Put(CustomerMotherObject.ValidCustomer().PublicId, CustomerMotherObject.ValidCustomerInput());

            var contentResult = (AcceptedResult)result;

            Assert.IsType<AcceptedResult>(contentResult);
            Assert.Equal(202, contentResult.StatusCode);
        }

        [Fact]
        public async Task DeleteCustomerValidTest()
        {
            var result = await _customersController.Delete(CustomerMotherObject.ValidCustomer().PublicId);

            var contentResult = (NoContentResult)result;

            Assert.IsType<NoContentResult>(contentResult);
            Assert.Equal(204, contentResult.StatusCode);
        }

        [Fact]
        public async Task ListCustomersTest()
        {
            var result = await _customersController.Get(new CustomerFilter());

            var contentResult = (OkObjectResult)result;
            var customersPagedList = (PagedList<CustomerOutput>)contentResult.Value;

            Assert.Equal(4, customersPagedList.Count);
            Assert.Equal(200, contentResult.StatusCode);
        }


        private void SetupServiceMock()
        {
            var customerServiceMock = new Mock<ICustomerService>();
            var customerList = new List<Customer>
            {
                CustomerMotherObject.ValidCustomer(),
                CustomerMotherObject.ValidCustomer(),
                CustomerMotherObject.ValidCustomer(),
                CustomerMotherObject.ValidCustomer()
            };

            var customerListPaginated = new PagedList<Customer>(customerList, 4);

            customerServiceMock.Setup(x => x.FindByIdAsync(CustomerMotherObject.ValidCustomer().PublicId)).ReturnsAsync(CustomerMotherObject.ValidCustomer());
            customerServiceMock.Setup(x => x.AddAsync(It.IsAny<Customer>())).ReturnsAsync(CustomerMotherObject.ValidCustomer());
            customerServiceMock.Setup(x => x.UpdateAsync(It.IsAny<Customer>())).Verifiable();
            customerServiceMock.Setup(x => x.DeleteAsync(CustomerMotherObject.ValidCustomer().PublicId)).Verifiable();
            customerServiceMock.Setup(x => x.ListPaginate(It.IsAny<CustomerFilter>())).
                ReturnsAsync(customerListPaginated);

            _customersController = new CustomersController(customerServiceMock.Object, _mapper);
        }
    }
}
