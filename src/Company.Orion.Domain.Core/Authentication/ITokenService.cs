using System.Security.Claims;
using Company.Orion.Domain.Core.Entities;

namespace Company.Orion.Domain.Core.Authentication;

public sealed record AccessToken(string Token, DateTime ExpiresAt);

/// <summary>
/// Creates and validates the access tokens (JWT) of the API. Implemented by the host
/// (<c>JwtTokenService</c>) so the application layer does not depend on a JWT library.
/// </summary>
public interface ITokenService
{
    AccessToken CreateAccessToken(User user);

    /// <summary>
    /// Validates the signature, issuer and audience of a token **ignoring its lifetime** — used by the
    /// refresh token flow, where the access token is expected to be expired.
    /// </summary>
    /// <returns>The principal of the token, or <c>null</c> when the token is not trusted.</returns>
    Task<ClaimsPrincipal?> ValidateExpiredAccessTokenAsync(string? token);
}
