using Asp.Versioning;
using Company.Orion.Api.Configuration;
using Company.Orion.Api.Controllers.Base;
using Company.Orion.Api.Models;
using Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithCredentials;
using Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithRefreshToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Company.Orion.Api.Controllers.V1;

/// <summary>
/// Authentication: issues and renews the JWT used by the other endpoints.
/// </summary>
[ApiVersion(1.0)]
[Route("[controller]")]
[AllowAnonymous]
[EnableRateLimiting(RateLimitingConfiguration.AuthPolicy)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
public class AuthController(IMediator mediator) : ApiController(mediator)
{
    /// <summary>
    /// Authenticates a user with e-mail and password.
    /// </summary>
    /// <remarks>
    /// Send the returned token as <c>Authorization: Bearer {token}</c>. When it expires, use
    /// <c>POST /Auth/RefreshToken</c> with the expired token and the refresh token to get a new pair.
    /// </remarks>
    /// <response code="200">A success response with a Token, Refresh Token and expiration date.</response>
    /// <response code="400">The request is invalid.</response>
    /// <response code="401">Invalid credentials.</response>
    /// <response code="429">Too many authentication attempts from the same client, try again later.</response>
    [HttpPost("Login")]
    [ProducesResponseType(typeof(UserApiTokenModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginWithCredentialsRequest loginWithCredentialsRequest)
    {
        return AuthorizeUser(await Mediator.Send(loginWithCredentialsRequest));
    }

    /// <summary>
    /// Renews an expired token using the refresh token.
    /// </summary>
    /// <remarks>
    /// The refresh token is rotated: the informed one is revoked and a new one is returned.
    /// </remarks>
    /// <response code="200">A success response with a new Token, Refresh Token and expiration date.</response>
    /// <response code="400">The request is invalid.</response>
    /// <response code="401">The token or the refresh token is invalid or expired.</response>
    /// <response code="429">Too many authentication attempts from the same client, try again later.</response>
    [HttpPost("RefreshToken")]
    [ProducesResponseType(typeof(UserApiTokenModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] LoginWithRefreshTokenRequest loginWithRefreshTokenRequest)
    {
        return AuthorizeUser(await Mediator.Send(loginWithRefreshTokenRequest));
    }

    private OkObjectResult AuthorizeUser(LoginWithCredentialsResponse loginWithCredentialsResponse)
    {
        return Ok(
            new UserApiTokenModel
            {
                Token = loginWithCredentialsResponse.Token,
                Expiration = loginWithCredentialsResponse.Expiration,
                RefreshToken = loginWithCredentialsResponse.RefreshToken
            });
    }
}
