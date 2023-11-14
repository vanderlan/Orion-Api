using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Orion.Test.Integration.Setup;

public class IntegrationTestsBootstrapper : IClassFixture<IntegrationTestsFixture>, IDisposable
{
    protected readonly ServiceProvider ServiceProvider;

    public IntegrationTestsBootstrapper(IntegrationTestsFixture fixture)
    {
        ServiceProvider = fixture.ServiceProvider;
    }

    private bool _disposedValue ;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            _disposedValue = true;
        }
    }

    ~IntegrationTestsBootstrapper()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
