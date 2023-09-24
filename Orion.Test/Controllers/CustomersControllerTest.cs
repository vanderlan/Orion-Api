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
    public class CustomersControllerTestTest : BaseControllerTest
    {
        private CustomersController _customersController;

        public CustomersControllerTestTest()
        {
            SetupServiceMock();
        }

        [Fact]
        public async Task GetCustomerById_WithValidId_ReturnsValidCustomer()
        {
            var result = await _customersController.Get(CustomerMotherObject.ValidCustomer().PublicId);

            var contentResult = (OkObjectResult) result;
            var customer = (CustomerOutput) contentResult.Value;

            Assert.IsType<OkObjectResult>(contentResult);
            Assert.Equal(200, contentResult.StatusCode);

            Assert.IsType<CustomerOutput>(contentResult.Value);
            Assert.Equal(CustomerMotherObject.ValidCustomer().Name, customer.Name);
        }

        [Fact]
        public async Task PostCustomer_WithValidData_CreateNewCustomer()
        {
            var result = await _customersController.Post(CustomerMotherObject.ValidCustomerInput());

            var contentResult = (CreatedResult) result;
            var contentResultObject = (CustomerOutput)contentResult.Value;

            Assert.IsType<CreatedResult>(contentResult);
            Assert.Equal(201, contentResult.StatusCode);
            Assert.NotNull(contentResult.Value);
            Assert.Equal(CustomerMotherObject.ValidCustomerInput().Name, contentResultObject.Name);
        }

        [Fact]
        public async Task PutCustomer_WithValidData_EditCustomer()
        {
            var result = await _customersController.Put(CustomerMotherObject.ValidCustomer().PublicId, CustomerMotherObject.ValidCustomerInput());

            var contentResult = (AcceptedResult)result;

            Assert.IsType<AcceptedResult>(contentResult);
            Assert.Equal(202, contentResult.StatusCode);
        }

        [Fact]
        public async Task DeleteCustomer_WithValidId_DeleteCustomer()
        {
            var result = await _customersController.Delete(CustomerMotherObject.ValidCustomer().PublicId);

            var contentResult = (NoContentResult)result;

            Assert.IsType<NoContentResult>(contentResult);
            Assert.Equal(204, contentResult.StatusCode);
        }

        [Fact]
        public async Task GetCustomers_WithPagination_ReturnsAllCustomers()
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

            _customersController = new CustomersController(customerServiceMock.Object, Mapper);
        }
    }
}
