using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Orion.Domain.Core.Exceptions;
using Orion.Domain.Core.Services.Interfaces;
using Orion.Test.Configuration;
using Xunit;
using Orion.Test.Domain.Services.BaseService;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Filters;
using Orion.Test.Faker;

namespace Orion.Test.Domain.Services;

public class CustomerServiceTest : BaseServiceTest
{
    public CustomerServiceTest(DependencyInjectionSetupFixture fixture) : base(fixture)
    {

    }

    [Fact]
    public async Task AddAsync_WithValidData_AddCustomerAsSuccess()
    {
        //arrange
        using var scope = ServiceProvider.CreateScope();
        var customerService = scope.ServiceProvider.GetService<ICustomerService>();

        var customer = CustomerFaker.Get();

        //act
        var customerSaved = await customerService.AddAsync(customer);
        var customerFound = await customerService.FindByIdAsync(customerSaved.PublicId);

        //assert
        Assert.NotNull(customerFound);
        Assert.Equal(customer.Name, customerFound.Name);
        Assert.True(customerFound.CustomerId > 0);
    }

    [Fact]
    public async Task DeleteAsync_WithExistantId_RemoveCustomerAsSuccess()
    {
        //arrange
        using var scope = ServiceProvider.CreateScope();
        var customerService = scope.ServiceProvider.GetService<ICustomerService>();
        var customer = CustomerFaker.Get();

        //act
        var customerSaved = await customerService.AddAsync(customer);
        var customerFound = await customerService.FindByIdAsync(customerSaved.PublicId);

        Assert.NotNull(customerFound);

        await customerService.DeleteAsync(customerFound.PublicId);

        var customerDeleted = await customerService.FindByIdAsync(customerSaved.PublicId);

        Assert.Null(customerDeleted);
    }

    [Fact]
    public async Task DeleteAsync_NotFound_ThrowsNotFoundException()
    {
        //arrange
        using var scope = ServiceProvider.CreateScope();
        var customerService = scope.ServiceProvider.GetService<ICustomerService>();

        //act & assert
        await Assert.ThrowsAsync<NotFoundException>(() => customerService.DeleteAsync("invalid_id"));
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdateCustomerAsSuccess()
    {
        //arrange
        using var scope = ServiceProvider.CreateScope();
        var customerService = scope.ServiceProvider.GetService<ICustomerService>();

        var customerSaved = await customerService.AddAsync(CustomerFaker.Get());
        var customerFound = await customerService.FindByIdAsync(customerSaved.PublicId);

        //act
        customerFound.Name = "Jane";
        await customerService.UpdateAsync(customerFound);
        await customerService.FindByIdAsync(customerSaved.PublicId);

        var customerEdited = await customerService.FindByIdAsync(customerSaved.PublicId);

        //assert
        Assert.Equal(customerFound.Name, customerEdited.Name);
    }

    [Fact]
    public async Task ListPaginateAsync_WithEmptyFilter_GetAllfCustomers()
    {
        //arrange
        using var scope = ServiceProvider.CreateScope();
        var customerService = scope.ServiceProvider.GetService<ICustomerService>();

        var customer = CustomerFaker.Get();
        var customer2 = CustomerFaker.Get();

        //act
        var customerSaved = await customerService.AddAsync(customer);
        var customerSaved2 = await customerService.AddAsync(customer2);

        var customerList = await customerService.ListPaginateAsync(new BaseFilter<Customer> { Quantity = 99 });

        //aseert
        Assert.NotNull(customerList);
        Assert.Contains(customerList.Items.Select(x => x.PublicId), x => x.Equals(customerSaved.PublicId));
        Assert.Contains(customerList.Items.Select(x => x.PublicId), x => x.Equals(customerSaved2.PublicId));
    }

    [Fact]
    public async Task ListPaginateAsync_WithFilterByName_GetAllMatchedCustomers()
    {
        //arrange
        using var scope = ServiceProvider.CreateScope();
        var customerService = scope.ServiceProvider.GetService<ICustomerService>();

        var customer = CustomerFaker.Get();
        var customer2 = CustomerFaker.Get();

        //act
        var customerSaved = await customerService.AddAsync(customer);
        var customerSaved2 = await customerService.AddAsync(customer2);

        var customerList = await customerService.ListPaginateAsync(new BaseFilter<Customer> { Query = customer.Name });

        //assert
        Assert.NotNull(customerList);
        Assert.True(customerList.Items.Where(x => x.Name.Equals(customer.Name)).Any());
        Assert.False(customerList.Items.Where(x => x.Name.Equals(customer2.Name)).Any());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(8)]
    [InlineData(12)]
    public async Task ListPaginateAsync_WithConfiguredQuantity_ReturnsListWithTheExpctedQuantity(int expectedQuantity)
    {
        //arrange
        using var scope = ServiceProvider.CreateScope();
        var customerService = scope.ServiceProvider.GetService<ICustomerService>();

        for (int i = 0; i < expectedQuantity; i++)
            await customerService.AddAsync(CustomerFaker.Get());

        //act
        var customerList = await customerService.ListPaginateAsync(new BaseFilter<Customer> { Quantity = expectedQuantity });

        //assert
        Assert.NotNull(customerList);
        Assert.Equal(expectedQuantity, customerList.Items.Count);
    }
}