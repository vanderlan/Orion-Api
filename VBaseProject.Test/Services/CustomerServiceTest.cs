using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using VBaseProject.Entities.Filter;
using VBaseProject.Service.Exceptions;
using VBaseProject.Service.Interfaces;
using VBaseProject.Test.Configuration;
using VBaseProject.Test.MotherObjects;
using VBaseProject.Test.Services.BaseService;
using Xunit;

namespace VBaseProject.Test.Services
{
    public class CustomerServiceTest : BaseServiceTest
	{
		public CustomerServiceTest(DependencyInjectionSetupFixture fixture) : base(fixture)
		{

		}

		[Fact]
		public async Task AddValidCustomerTest()
		{
			using var scope = _serviceProvider.CreateScope();
			var _customerService = scope.ServiceProvider.GetService<ICustomerService>();

			var customerSaved = await _customerService.AddAsync(CustomerMotherObject.ValidCustomer());
			var customerFound = await _customerService.FindByIdAsync(customerSaved.PublicId);

			Assert.NotNull(customerFound);
			Assert.Equal(CustomerMotherObject.ValidCustomer().Name, customerFound.Name);
			Assert.Equal(customerFound.PhoneNumber, CustomerMotherObject.ValidCustomer().PhoneNumber);
		}


		[Fact]
		public async Task RemoveCustomerTest()
		{
			using var scope = _serviceProvider.CreateScope();
			var _customerService = scope.ServiceProvider.GetService<ICustomerService>();

			var customerSaved = await _customerService.AddAsync(CustomerMotherObject.ValidCustomer());
			var customerFound = await _customerService.FindByIdAsync(customerSaved.PublicId);

			Assert.NotNull(customerFound);

			await _customerService.DeleteAsync(customerFound.PublicId);

			var customerDeleted = await _customerService.FindByIdAsync(customerSaved.PublicId);

			Assert.Null(customerDeleted);
		}
		[Fact]
		public async Task RemoveCustomerNotFoundTest()
		{
			using var scope = _serviceProvider.CreateScope();
			var _customerService = scope.ServiceProvider.GetService<ICustomerService>();

			await Assert.ThrowsAsync<NotFoundException>(() => _customerService.DeleteAsync("invalid_id"));
		}

		[Fact]
		public async Task EditCustomerTest()
		{
			using var scope = _serviceProvider.CreateScope();
			var _customerService = scope.ServiceProvider.GetService<ICustomerService>();

			var customerSaved = await _customerService.AddAsync(CustomerMotherObject.ValidCustomer());
			var customerFound = await _customerService.FindByIdAsync(customerSaved.PublicId);

			Assert.NotNull(customerFound);

			customerFound.Name = "Jane";
			customerFound.PhoneNumber = "789879879797";
			customerFound.Address = "Wall Street";

			await _customerService.UpdateAsync(customerFound);
			await _customerService.FindByIdAsync(customerSaved.PublicId);

			var customerEdited = await _customerService.FindByIdAsync(customerSaved.PublicId);

			Assert.Equal(customerFound.Name, customerEdited.Name);
			Assert.Equal(customerFound.PhoneNumber, customerEdited.PhoneNumber);
			Assert.Equal(customerFound.Address, customerEdited.Address);
		}

		[Fact]
		public async Task ListCustomerPaginatedTest()
		{
			using var scope = _serviceProvider.CreateScope();
			var _customerService = scope.ServiceProvider.GetService<ICustomerService>();

			var customerSaved = await _customerService.AddAsync(CustomerMotherObject.ValidCustomer());
			var customerSaved2 = await _customerService.AddAsync(CustomerMotherObject.ValidCustomer2());

			var customerList = await _customerService.ListPaginate(new CustomerFilter { });

			Assert.NotNull(customerList);

			Assert.True(customerList.Items.Where(x => x.Name.Equals(CustomerMotherObject.ValidCustomer().Name)).Any());
			Assert.True(customerList.Items.Where(x => x.Name.Equals(CustomerMotherObject.ValidCustomer2().Name)).Any());
		}

		[Fact]
		public async Task ListCustomerPaginatedFilterByNameTest()
		{
			using var scope = _serviceProvider.CreateScope();
			var _customerService = scope.ServiceProvider.GetService<ICustomerService>();

			var customerSaved = await _customerService.AddAsync(CustomerMotherObject.ValidCustomer());
			var customerSaved2 = await _customerService.AddAsync(CustomerMotherObject.ValidCustomer2());

			var customerList = await _customerService.ListPaginate(new CustomerFilter { Query = CustomerMotherObject.ValidCustomer().Name });

			Assert.NotNull(customerList);

			Assert.True(customerList.Items.Where(x => x.Name.Equals(CustomerMotherObject.ValidCustomer().Name)).Any());
			Assert.False(customerList.Items.Where(x => x.Name.Equals(CustomerMotherObject.ValidCustomer2().Name)).Any());
		}

		[Fact]
		public async Task ListCustomerPaginatedQuantityTest()
		{
			const int expectedQuantity = 1;

			using var scope = _serviceProvider.CreateScope();
			var _customerService = scope.ServiceProvider.GetService<ICustomerService>();

			await _customerService.AddAsync(CustomerMotherObject.ValidCustomer());
			await _customerService.AddAsync(CustomerMotherObject.ValidCustomer2());

			var customerList = await _customerService.ListPaginate(new CustomerFilter { Quantity = expectedQuantity });

			Assert.NotNull(customerList);
			Assert.Equal(expectedQuantity, customerList.Items.Count);
		}
	}
}