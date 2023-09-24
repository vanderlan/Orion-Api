using System.Net;
using System.Threading.Tasks;
using Orion.Test.Configuration;
using Xunit;

namespace Orion.Test.API
{
    public class HealthCheckApiTest: ApiTestInitializer
    {
        [Fact]
        public async Task HealthCheckConfigurationTest()
        {
            Setup();

            var successMessageService = "Healthy";
            var result = await Client.GetAsync("/health-check");

            var content = await result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(successMessageService, content);
        }
    }
}
