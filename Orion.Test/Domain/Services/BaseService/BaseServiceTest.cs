using Microsoft.Extensions.DependencyInjection;
using Orion.Test.Configuration;
using Xunit;

namespace Orion.Test.Domain.Services.BaseService
{
    public class BaseServiceTest : IClassFixture<DependencyInjectionSetupFixture>
    {
        protected readonly ServiceProvider ServiceProvider;

        public BaseServiceTest(DependencyInjectionSetupFixture fixture)
        {
            ServiceProvider = fixture.ServiceProvider;
        }
    }
}
