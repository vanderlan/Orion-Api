using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using VBaseProject.Api.AutoMapper.Output;
using VBaseProject.Entities.ValueObjects.Pagination;
using VBaseProject.Test.TestConfiguration;
using Xunit;

namespace VBaseProject.Test.Integrations
{
    public class CustomersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        public readonly HttpClient _client;

        public CustomersControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetCustomersTest()
        {
            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync("/api/Customers");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var customers = JsonConvert.DeserializeObject<PagedList<CustomerOutput>>(stringResponse);

            Assert.True(customers.Items.Any());
        }
    }
}
