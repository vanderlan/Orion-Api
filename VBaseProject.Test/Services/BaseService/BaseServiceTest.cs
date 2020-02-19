using Microsoft.Extensions.DependencyInjection;
using VBaseProject.Test.Configuration;
using Xunit;

namespace VBaseProject.Test.Services.BaseService
{
	public class BaseServiceTest : IClassFixture<DependencyInjectionSetupFixture>
	{
		protected readonly ServiceProvider _serviceProvider;

		public BaseServiceTest(DependencyInjectionSetupFixture fixture)
		{
			_serviceProvider = fixture.ServiceProvider;
		}
	}
}
