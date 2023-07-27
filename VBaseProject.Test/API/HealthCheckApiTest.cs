using System.Net;
using System.Threading.Tasks;
using VBaseProject.Test.Configuration;
using Xunit;

namespace VBaseProject.Test.Api
{
    public class HealthCheckApiTest: ApiTestInitializer
    {
        [Fact]
        public async Task GetAsync_HealthCheck_ReturnsHealthy()
        {
            Setup();

            var successMessageService = "Healthy";
            var result = await _client.GetAsync("/health-check");

            var content = await result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(successMessageService, content);
        }
    }
}
