using Newtonsoft.Json;
using Company.Orion.Api.Models;
using Company.Orion.Domain.Core.Entities;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithCredentials;
using Xunit;

namespace Company.Orion.Test.Integration.Setup;

public abstract class IntegrationTestsBootstrapper(IntegrationTestsFixture fixture)
    : IClassFixture<IntegrationTestsFixture>, IDisposable
{
    protected readonly HttpClient HttpClient = fixture.HttpClient;
    protected readonly HttpClient AuthenticatedHttpClient = fixture.AuthenticatedHttpClient;
    protected readonly User DefaultSystemUser = fixture.DefaultSystemUser;
    protected readonly IntegrationTestsFixture IntegrationTestsFixture = fixture;
    
    protected IServiceProvider ServiceProvider { get; private set; } = fixture.ServiceProvider;

    protected void LoginWithDefaultUser()
    {
        var tokenResult = AuthUser(DefaultSystemUser.Email, "123");

        AuthenticatedHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Token);
    }

    protected UserApiTokenModel AuthUser(string email, string password)
    {
        var result = HttpClient.PostAsync("/Auth/Login", GetStringContent(
                new LoginWithCredentialsRequest
                {
                    Email = email,
                    Password = password
                }))
            .GetAwaiter().GetResult();

        var content = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        return JsonConvert.DeserializeObject<UserApiTokenModel>(content);
    }

    protected static StringContent GetStringContent(object obj)
    {
        return new StringContent(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");
    }

    private bool _disposedValue;

    private void Dispose(bool disposing)
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

    protected static async Task<T> GetResultContentAsync<T>(HttpResponseMessage httpResponse)
    {
        var content = await httpResponse.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<T>(content);
    }
}
