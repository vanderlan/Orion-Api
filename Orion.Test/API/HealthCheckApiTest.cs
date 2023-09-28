using System.Net;
using System.Threading.Tasks;
using Orion.Test.Configuration;
using Xunit;

namespace Orion.Test.API
{
    public class HealthCheckApiTest: ApiTestInitializer
    {
        [Fact]
        public async Task GetAsync_HealthCheck_ReturnsHealthy()
        {
            //arrange
            var successMessageService = "Healthy";

            //act
            var result = await Client.GetAsync("/health-check");

            //assert
            var content = await result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(successMessageService, content);
        }
    }
}
