using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Orion.Api.Models;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Repositories.UnitOfWork;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Orion.Test.Api.Setup;

public abstract class ApiTestsBootstrapper : IClassFixture<ApiTestsFixture>, IDisposable
{
    protected readonly HttpClient HttpClient;
    protected readonly HttpClient AuthenticatedHttpClient;
    protected readonly User DefaultSystemUser;
    protected IServiceProvider ServiceProvider { get; private set; }

    public ApiTestsBootstrapper(ApiTestsFixture fixture)
    {
        HttpClient = fixture.HttpClient;
        AuthenticatedHttpClient = fixture.AuthenticatedHttpClient;
        DefaultSystemUser = fixture.DefaultSystemUser;
        ServiceProvider = fixture.ServiceProvider;
    }

    protected void AuthUser()
    {
        var result = HttpClient.PostAsync("/api/Auth/Login", GetStringContent(
            new UserLoginModel
            {
                Email = DefaultSystemUser.Email,
                Password = "123"
            }))
            .GetAwaiter().GetResult();

        var content = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        var tokenResult = JsonConvert.DeserializeObject<UserApiTokenModel>(content);

        AuthenticatedHttpClient.DefaultRequestHeaders.Authorization = new("Bearer", tokenResult.Token);
    }

    protected static StringContent GetStringContent(object obj)
    {
        return new StringContent(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");
    }

    private bool _disposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            _disposedValue = true;
        }
    }

    ~ApiTestsBootstrapper()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected static async Task<T> GetResultContentAsync<T>(HttpResponseMessage httpResponse)
    {
        var content = await httpResponse.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<T>(content);
    }
}
