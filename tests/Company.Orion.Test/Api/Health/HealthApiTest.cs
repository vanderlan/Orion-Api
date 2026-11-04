using System.Net;
using System.Threading.Tasks;
using Company.Orion.Api.Configuration;
using Company.Orion.Test.Integration.Setup;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Company.Orion.Test.Api.Health;

public class HealthApiTest(IntegrationTestsFixture fixture) : IntegrationTestsBootstrapper(fixture)
{
    [Fact]
    public async Task GetLiveness_ReturnsHealthyWithoutCheckingDependencies()
    {
        //arrange & act
        var httpResponse = await HttpClient.GetAsync(HealthCheckConfiguration.LivenessPath);

        //assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal("Healthy", await httpResponse.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task GetReadiness_WithTheDatabaseAvailable_ReturnsHealthyReport()
    {
        //arrange & act
        var httpResponse = await HttpClient.GetAsync(HealthCheckConfiguration.ReadinessPath);
        var report = JObject.Parse(await httpResponse.Content.ReadAsStringAsync());

        //assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal("Healthy", report.Value<string>("status"));
        Assert.NotEmpty(report["entries"]);
    }

    [Fact]
    public async Task GetHealth_ReturnsTheFullReport()
    {
        //arrange & act
        var httpResponse = await HttpClient.GetAsync(HealthCheckConfiguration.BasePath);

        //assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }
}
