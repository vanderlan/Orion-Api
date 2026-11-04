using System.Security.Claims;
using System.Text;
using Company.Orion.Domain.Core.Authentication;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Company.Orion.Api.Security;

/// <summary>
/// JWT implementation of <see cref="ITokenService"/> based on <see cref="JsonWebTokenHandler"/>
/// (the handler recommended by Microsoft, also used by the JwtBearer middleware), signed with HMAC-SHA512.
/// </summary>
public sealed class JwtTokenService(IOptions<JwtOptions> jwtOptions, TimeProvider timeProvider) : ITokenService
{
    private readonly JwtOptions _options = jwtOptions.Value;
    private readonly JsonWebTokenHandler _handler = new();

    public AccessToken CreateAccessToken(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var expiresAt = now.AddMinutes(_options.TokenExpirationMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            IssuedAt = now,
            NotBefore = now,
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(CreateSigningKey(_options), SecurityAlgorithms.HmacSha512),
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.Name),
                new Claim(ClaimTypes.Sid, user.PublicId),
                new Claim(ClaimTypes.Role, user.Profile.Description())
            ])
        };

        return new AccessToken(_handler.CreateToken(descriptor), expiresAt);
    }

    public async Task<ClaimsPrincipal> ValidateExpiredAccessTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var result = await _handler.ValidateTokenAsync(token, CreateValidationParameters(_options, validateLifetime: false));

        return result.IsValid ? new ClaimsPrincipal(result.ClaimsIdentity) : null;
    }

    /// <summary>
    /// Single source of truth for the validation rules, shared with the JwtBearer middleware.
    /// </summary>
    public static TokenValidationParameters CreateValidationParameters(JwtOptions options, bool validateLifetime = true)
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = options.Issuer,
            ValidateAudience = true,
            ValidAudience = options.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = CreateSigningKey(options),
            ValidAlgorithms = [SecurityAlgorithms.HmacSha512],
            ValidateLifetime = validateLifetime,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.GivenName
        };
    }

    private static SymmetricSecurityKey CreateSigningKey(JwtOptions options)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SymmetricSecurityKey));
    }
}
