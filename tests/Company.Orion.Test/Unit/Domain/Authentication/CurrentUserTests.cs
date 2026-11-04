using System.Security.Claims;
using Company.Orion.Domain.Core.Authentication;
using Microsoft.AspNetCore.Http;
using Xunit;
using static Company.Orion.Domain.Core.Authentication.AuthorizationConfiguration;

namespace Company.Orion.Test.Unit.Domain.Authentication;

public class CurrentUserTests
{
    [Fact]
    public void Properties_WithAnAuthenticatedUser_ReturnTheClaims()
    {
        //arrange
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.GivenName, "Orion Admin User"),
            new Claim(ClaimTypes.Sid, "16537902-1ca7-49ca-82e5-0be137f9aeeb"),
            new Claim(ClaimTypes.Email, "adm@orion.com"),
            new Claim(ClaimTypes.Role, Roles.Admin)
        ], "Test");

        var currentUser = new CurrentUser(new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        });

        //act & assert
        Assert.True(currentUser.IsAuthenticated());
        Assert.Equal("Orion Admin User", currentUser.Name);
        Assert.Equal("16537902-1ca7-49ca-82e5-0be137f9aeeb", currentUser.Id);
        Assert.Equal("adm@orion.com", currentUser.Email);
        Assert.Equal(Roles.Admin, currentUser.Profile);
        Assert.Contains("adm@orion.com", currentUser.ToString());
    }

    [Fact]
    public void Properties_WithoutHttpContext_ReturnAnonymous()
    {
        //arrange
        var currentUser = new CurrentUser(new HttpContextAccessor());

        //act & assert
        Assert.False(currentUser.IsAuthenticated());
        Assert.Equal(string.Empty, currentUser.Name);
        Assert.Equal(string.Empty, currentUser.Id);
        Assert.Equal(string.Empty, currentUser.Email);
        Assert.Equal(string.Empty, currentUser.Profile);
        Assert.Equal("Anonymous User", currentUser.ToString());
    }

    [Fact]
    public void Properties_WithAnUnauthenticatedIdentity_ReturnEmptyValues()
    {
        //arrange: claims present but identity not authenticated (no authentication type)
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Email, "adm@orion.com")]);

        var currentUser = new CurrentUser(new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        });

        //act & assert
        Assert.False(currentUser.IsAuthenticated());
        Assert.Equal(string.Empty, currentUser.Email);
    }
}
