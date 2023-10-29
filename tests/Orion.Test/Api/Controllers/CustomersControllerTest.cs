using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orion.Api.AutoMapper.Output;
using Xunit;
using Orion.Test.Api.Controllers.BaseController;
using Orion.Domain.Core.Filters;
using Orion.Domain.Core.ValueObjects.Pagination;

namespace Orion.Test.Api.Controllers;

public class CustomersControllerTestTest : BaseControllerTest
{
    private CustomersController _customersController;
    private readonly Customer _validCustomer = CustomerFaker.Get();
    private readonly CustomerInput _validCustomerInput = CustomerFaker.GetInput();

    public CustomersControllerTestTest()
    {
        SetupServiceMock();
    }

    [Fact]
    public async Task GetCustomerById_WithValidId_ReturnsValidCustomer()
    {
        //arrange & act
        var result = await _customersController.Get(_validCustomer.PublicId);

        var contentResult = (OkObjectResult)result;
        var customer = (CustomerOutput)contentResult.Value;

        //assert
        Assert.IsType<OkObjectResult>(contentResult);
        Assert.Equal(200, contentResult.StatusCode);

        Assert.IsType<CustomerOutput>(contentResult.Value);
        Assert.Equal(_validCustomer.Name, customer.Name);
    }

    [Fact]
    public async Task PostCustomer_WithValidData_CreateNewCustomer()
    {
        //arrange & act
        var result = await _customersController.Post(_validCustomerInput);

        var contentResult = (CreatedResult)result;
        var contentResultObject = (CustomerOutput)contentResult.Value;

        //assert
        Assert.IsType<CreatedResult>(contentResult);
        Assert.Equal(201, contentResult.StatusCode);
        Assert.NotNull(contentResult.Value);
        Assert.Equal(_validCustomer.Name, contentResultObject.Name);
    }

    [Fact]
    public async Task PutCustomer_WithValidData_EditCustomer()
    {
        //arrange & act
        var result = await _customersController.Put(_validCustomer.PublicId, CustomerFaker.GetInput());

        var contentResult = (AcceptedResult)result;

        //assert
        Assert.IsType<AcceptedResult>(contentResult);
        Assert.Equal(202, contentResult.StatusCode);
    }

    [Fact]
    public async Task DeleteCustomer_WithValidId_DeleteCustomer()
    {
        //arrange & act
        var result = await _customersController.Delete(_validCustomer.PublicId);

        var contentResult = (NoContentResult)result;

        //assert
        Assert.IsType<NoContentResult>(contentResult);
        Assert.Equal(204, contentResult.StatusCode);
    }

    [Fact]
    public async Task GetCustomers_WithPagination_ReturnsAllCustomers()
    {
        //arrange
        var expectedCount = 4;

        //act
        var result = await _customersController.Get(new BaseFilter<Customer>());

        var contentResult = (OkObjectResult)result;
        var customersPagedList = (PagedList<CustomerOutput>)contentResult.Value;

        //assert
        Assert.Equal(expectedCount, customersPagedList.Count);
        Assert.Equal(200, contentResult.StatusCode);
    }

    private void SetupServiceMock()
    {
        var customerServiceMock = new Mock<ICustomerService>();
        var customerList = new List<Customer>
        {
            _validCustomer,
            CustomerFaker.Get(),
            CustomerFaker.Get(),
            CustomerFaker.Get()
        };

        var customerListPaginated = new PagedList<Customer>(customerList, 4);

        customerServiceMock.Setup(x => x.FindByIdAsync(_validCustomer.PublicId)).ReturnsAsync(_validCustomer);
        customerServiceMock.Setup(x => x.AddAsync(It.Is<Customer>(x => x.Name == _validCustomerInput.Name))).ReturnsAsync(_validCustomer);
        customerServiceMock.Setup(x => x.UpdateAsync(It.IsAny<Customer>())).Verifiable();
        customerServiceMock.Setup(x => x.DeleteAsync(CustomerFaker.Get().PublicId)).Verifiable();
        customerServiceMock.Setup(x => x.ListPaginateAsync(It.IsAny<BaseFilter<Customer>>())).
            ReturnsAsync(customerListPaginated);

        _customersController = new CustomersController(customerServiceMock.Object, Mapper);
    }
}
