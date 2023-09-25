using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Orion.Entities.Filter;
using Orion.Domain.Exceptions;
using Orion.Domain.Interfaces;
using Orion.Test.Configuration;
using Orion.Test.MotherObjects;
using Orion.Test.Services.BaseService;
using Xunit;

namespace Orion.Test.Services
{
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

			//act
			var customerSaved = await customerService.AddAsync(CustomerMotherObject.ValidCustomer());
			var customerFound = await customerService.FindByIdAsync(customerSaved.PublicId);

			//assert
			Assert.NotNull(customerFound);
			Assert.Equal(CustomerMotherObject.ValidCustomer().Name, customerFound.Name);
			Assert.True(customerFound.CustomerId > 0);
		}

		[Fact]
		public async Task DeleteAsync_WithExistantId_RemoveCustomerAsSuccess()
		{
            //arrange
            using var scope = ServiceProvider.CreateScope();
			var customerService = scope.ServiceProvider.GetService<ICustomerService>();

            //act
            var customerSaved = await customerService.AddAsync(CustomerMotherObject.ValidCustomer());
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

			var customerSaved = await customerService.AddAsync(CustomerMotherObject.ValidCustomer());
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

            //act
            var customerSaved = await customerService.AddAsync(CustomerMotherObject.ValidCustomer());
			var customerSaved2 = await customerService.AddAsync(CustomerMotherObject.ValidCustomer2());

			var customerList = await customerService.ListPaginateAsync(new CustomerFilter { });

			//aseert
			Assert.NotNull(customerList);
			Assert.True(customerList.Items.Where(x => x.Name.Equals(CustomerMotherObject.ValidCustomer().Name)).Any());
			Assert.True(customerList.Items.Where(x => x.Name.Equals(CustomerMotherObject.ValidCustomer2().Name)).Any());
		}

		[Fact]
		public async Task ListPaginateAsync_WithFilterByName_GetAllMatchedCustomers()
		{
            //arrange
            using var scope = ServiceProvider.CreateScope();
			var customerService = scope.ServiceProvider.GetService<ICustomerService>();

            //act
            var customerSaved = await customerService.AddAsync(CustomerMotherObject.ValidCustomer());
			var customerSaved2 = await customerService.AddAsync(CustomerMotherObject.ValidCustomer2());

			var customerList = await customerService.ListPaginateAsync(new CustomerFilter { Query = CustomerMotherObject.ValidCustomer().Name });

			//assert
			Assert.NotNull(customerList);
			Assert.True(customerList.Items.Where(x => x.Name.Equals(CustomerMotherObject.ValidCustomer().Name)).Any());
			Assert.False(customerList.Items.Where(x => x.Name.Equals(CustomerMotherObject.ValidCustomer2().Name)).Any());
		}

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(3)]
		public async Task ListPaginateAsync_WithConfiguredQuantiry_ReturnsListWithTheExpctedQuantity(int expectedQuantity)
		{
            //arrange
			using var scope = ServiceProvider.CreateScope();
			var customerService = scope.ServiceProvider.GetService<ICustomerService>();

            //act
            await customerService.AddAsync(CustomerMotherObject.ValidCustomer());
			var customerList = await customerService.ListPaginateAsync(new CustomerFilter { Quantity = expectedQuantity });

			//assert
			Assert.NotNull(customerList);
			Assert.Equal(expectedQuantity, customerList.Items.Count);
		}
	}
}