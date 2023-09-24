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
		public async Task AddValidCustomerTest()
		{
			using var scope = ServiceProvider.CreateScope();
			var customerService = scope.ServiceProvider.GetService<ICustomerService>();

			var customerSaved = await customerService.AddAsync(CustomerMotherObject.ValidCustomer());
			var customerFound = await customerService.FindByIdAsync(customerSaved.PublicId);

			Assert.NotNull(customerFound);
			Assert.Equal(CustomerMotherObject.ValidCustomer().Name, customerFound.Name);
			Assert.True(customerFound.CustomerId > 0);
		}

		[Fact]
		public async Task RemoveCustomerTest()
		{
			using var scope = ServiceProvider.CreateScope();
			var customerService = scope.ServiceProvider.GetService<ICustomerService>();

			var customerSaved = await customerService.AddAsync(CustomerMotherObject.ValidCustomer());
			var customerFound = await customerService.FindByIdAsync(customerSaved.PublicId);

			Assert.NotNull(customerFound);

			await customerService.DeleteAsync(customerFound.PublicId);

			var customerDeleted = await customerService.FindByIdAsync(customerSaved.PublicId);

			Assert.Null(customerDeleted);
		}
		[Fact]
		public async Task RemoveCustomerNotFoundTest()
		{
			using var scope = ServiceProvider.CreateScope();
			var customerService = scope.ServiceProvider.GetService<ICustomerService>();

			await Assert.ThrowsAsync<NotFoundException>(() => customerService.DeleteAsync("invalid_id"));
		}

		[Fact]
		public async Task EditCustomerTest()
		{
			using var scope = ServiceProvider.CreateScope();
			var customerService = scope.ServiceProvider.GetService<ICustomerService>();

			var customerSaved = await customerService.AddAsync(CustomerMotherObject.ValidCustomer());
			var customerFound = await customerService.FindByIdAsync(customerSaved.PublicId);

			Assert.NotNull(customerFound);

			customerFound.Name = "Jane";

			await customerService.UpdateAsync(customerFound);
			await customerService.FindByIdAsync(customerSaved.PublicId);

			var customerEdited = await customerService.FindByIdAsync(customerSaved.PublicId);

			Assert.Equal(customerFound.Name, customerEdited.Name);
		}

		[Fact]
		public async Task ListCustomerPaginatedTest()
		{
			using var scope = ServiceProvider.CreateScope();
			var customerService = scope.ServiceProvider.GetService<ICustomerService>();

			var customerSaved = await customerService.AddAsync(CustomerMotherObject.ValidCustomer());
			var customerSaved2 = await customerService.AddAsync(CustomerMotherObject.ValidCustomer2());

			var customerList = await customerService.ListPaginate(new CustomerFilter { });

			Assert.NotNull(customerList);

			Assert.True(customerList.Items.Where(x => x.Name.Equals(CustomerMotherObject.ValidCustomer().Name)).Any());
			Assert.True(customerList.Items.Where(x => x.Name.Equals(CustomerMotherObject.ValidCustomer2().Name)).Any());
		}

		[Fact]
		public async Task ListCustomerPaginatedFilterByNameTest()
		{
			using var scope = ServiceProvider.CreateScope();
			var customerService = scope.ServiceProvider.GetService<ICustomerService>();

			var customerSaved = await customerService.AddAsync(CustomerMotherObject.ValidCustomer());
			var customerSaved2 = await customerService.AddAsync(CustomerMotherObject.ValidCustomer2());

			var customerList = await customerService.ListPaginate(new CustomerFilter { Query = CustomerMotherObject.ValidCustomer().Name });

			Assert.NotNull(customerList);

			Assert.True(customerList.Items.Where(x => x.Name.Equals(CustomerMotherObject.ValidCustomer().Name)).Any());
			Assert.False(customerList.Items.Where(x => x.Name.Equals(CustomerMotherObject.ValidCustomer2().Name)).Any());
		}

		[Fact]
		public async Task ListCustomerPaginatedQuantityTest()
		{
			const int expectedQuantity = 1;

			using var scope = ServiceProvider.CreateScope();
			var customerService = scope.ServiceProvider.GetService<ICustomerService>();

			await customerService.AddAsync(CustomerMotherObject.ValidCustomer());

			var customerList = await customerService.ListPaginate(new CustomerFilter { Quantity = expectedQuantity });

			Assert.NotNull(customerList);
			Assert.Equal(expectedQuantity, customerList.Items.Count);
		}
	}
}