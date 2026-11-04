using System.ComponentModel.DataAnnotations;

namespace Company.Orion.Domain.Core.Authentication;

/// <summary>
/// Section of appsettings that contains the JWT configuration. Validated at startup
/// (<c>ValidateOnStart</c>): the application refuses to run with an invalid/weak configuration.
/// </summary>
public sealed class JwtOptions
{
    public const string SectionName = "JwtOptions";

    /// <summary>Secret used to sign the tokens (HMAC-SHA512 requires at least 64 characters).</summary>
    [Required]
    [MinLength(64, ErrorMessage = "The JWT SymmetricSecurityKey must have at least 64 characters (HS512).")]
    public string SymmetricSecurityKey { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    /// <summary>Lifetime of the access token (JWT).</summary>
    [Range(1, 1440)]
    public int TokenExpirationMinutes { get; set; } = 15;

    /// <summary>Lifetime of the refresh token. Each rotation issues a new token with a full lifetime.</summary>
    [Range(1, 365)]
    public int RefreshTokenExpirationDays { get; set; } = 7;

    public TimeSpan RefreshTokenLifetime => TimeSpan.FromDays(RefreshTokenExpirationDays);
}
