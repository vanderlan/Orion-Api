using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Company.Orion.Api.Security;
using Company.Orion.Domain.Core.Authentication;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Test.Shared;
using Company.Orion.Test.Shared.Faker;
using Microsoft.Extensions.Options;
using Xunit;
using static Company.Orion.Domain.Core.Authentication.AuthorizationConfiguration;

namespace Company.Orion.Test.Unit.Api.Security;

public class JwtTokenServiceTests
{
    private static readonly DateTimeOffset Now = new(2025, 3, 21, 12, 0, 0, TimeSpan.Zero);

    private readonly JwtOptions _options = new()
    {
        SymmetricSecurityKey = "REPLACE-WITH-A-RANDOM-SECRET-OF-AT-LEAST-64-CHARACTERS-REQUIRED-FOR-HS512",
        Issuer = "Orion API Tests",
        Audience = "http://www.orion-api.com",
        TokenExpirationMinutes = 15
    };

    private readonly TestTimeProvider _timeProvider = new(Now);
    private readonly User _user = UserFaker.Get();
    private readonly JwtTokenService _tokenService;

    public JwtTokenServiceTests()
    {
        _tokenService = new JwtTokenService(Options.Create(_options), _timeProvider);
    }

    [Fact]
    public void CreateAccessToken_ReturnsASignedJwtExpiringAfterTheConfiguredMinutes()
    {
        //act
        var accessToken = _tokenService.CreateAccessToken(_user);

        //assert
        Assert.Equal(3, accessToken.Token.Split('.').Length);
        Assert.Equal(Now.UtcDateTime.AddMinutes(15), accessToken.ExpiresAt);
    }

    [Fact]
    public async Task ValidateExpiredAccessTokenAsync_WithATokenIssuedByTheService_ReturnsThePrincipalEvenWhenExpired()
    {
        //arrange
        var accessToken = _tokenService.CreateAccessToken(_user);

        _timeProvider.Advance(TimeSpan.FromHours(2));

        //act
        var principal = await _tokenService.ValidateExpiredAccessTokenAsync(accessToken.Token);

        //assert
        Assert.NotNull(principal);
        Assert.Equal(_user.Email, principal.FindFirst(ClaimTypes.Email)?.Value);
        Assert.Equal(_user.Name, principal.FindFirst(ClaimTypes.GivenName)?.Value);
        Assert.Equal(_user.PublicId, principal.FindFirst(ClaimTypes.Sid)?.Value);
        Assert.Equal(Roles.Admin, principal.FindFirst(ClaimTypes.Role)?.Value);
        Assert.True(principal.IsInRole(Roles.Admin));
    }

    [Fact]
    public async Task ValidateExpiredAccessTokenAsync_WithATokenSignedWithAnotherKey_ReturnsNull()
    {
        //arrange
        var otherOptions = new JwtOptions
        {
            SymmetricSecurityKey = "ANOTHER-RANDOM-SECRET-OF-AT-LEAST-64-CHARACTERS-REQUIRED-FOR-HS512-XXXXXX",
            Issuer = _options.Issuer,
            Audience = _options.Audience
        };

        var forgedToken = new JwtTokenService(Options.Create(otherOptions), _timeProvider).CreateAccessToken(_user);

        //act & assert
        Assert.Null(await _tokenService.ValidateExpiredAccessTokenAsync(forgedToken.Token));
    }

    [Fact]
    public async Task ValidateExpiredAccessTokenAsync_WithATokenOfAnotherIssuer_ReturnsNull()
    {
        //arrange
        var otherOptions = new JwtOptions
        {
            SymmetricSecurityKey = _options.SymmetricSecurityKey,
            Issuer = "Another API",
            Audience = _options.Audience
        };

        var foreignToken = new JwtTokenService(Options.Create(otherOptions), _timeProvider).CreateAccessToken(_user);

        //act & assert
        Assert.Null(await _tokenService.ValidateExpiredAccessTokenAsync(foreignToken.Token));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-jwt")]
    public async Task ValidateExpiredAccessTokenAsync_WithGarbage_ReturnsNull(string token)
    {
        Assert.Null(await _tokenService.ValidateExpiredAccessTokenAsync(token));
    }

    [Fact]
    public void CreateValidationParameters_ValidatesIssuerAudienceSignatureAndUsesTheRoleClaim()
    {
        //act
        var parameters = JwtTokenService.CreateValidationParameters(_options);

        //assert
        Assert.True(parameters.ValidateIssuer);
        Assert.True(parameters.ValidateAudience);
        Assert.True(parameters.ValidateIssuerSigningKey);
        Assert.True(parameters.ValidateLifetime);
        Assert.Equal(TimeSpan.Zero, parameters.ClockSkew);
        Assert.Equal(ClaimTypes.Role, parameters.RoleClaimType);
        Assert.False(JwtTokenService.CreateValidationParameters(_options, validateLifetime: false).ValidateLifetime);
    }
}
