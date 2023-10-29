using System.Net;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orion.Api.Configuration;
using Orion.Api.Controllers.Base;
using Orion.Api.Models;
using Orion.Application.Core.Commands.LoginWithCredentials;
using Orion.Application.Core.Commands.LoginWithRefreshToken;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Exceptions;
using Orion.Domain.Core.Extensions;
using Orion.Domain.Core.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Orion.Api.Controllers.V1;

[ApiVersion(1.0)]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ApiController
{
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;

    public AuthController(IUserService userService, IMediator mediator, IConfiguration configuration) : base(mediator)
    {
        _configuration = configuration;
        _userService = userService;
    }

    [HttpPost("Login")]
    [SwaggerResponse((int)HttpStatusCode.OK,"A success reponse with a Token, Refresh Token and expiration date" ,typeof(LoginWithCredentialsResponse))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest,"A error response with the error description" ,typeof(ExceptionResponse))]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized,"When the credetials are invalid")]
    public async Task<IActionResult> Login([FromBody] LoginWithCredentialsRequest loginWithCredentialsRequest)
    {
        var loginResponse = await Mediator.Send(loginWithCredentialsRequest);

        return await AuthorizeUser(loginResponse);
    }

    [HttpPost("RefreshToken")]
    [SwaggerResponse((int)HttpStatusCode.OK,"A success reponse with a Token, Refresh Token and expiration date" ,typeof(LoginWithRefreshTokenResponse))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest,"A error response with the error description" ,typeof(ExceptionResponse))]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized,"When the credetials are invalid")]
    public async Task<IActionResult> RefreshToken([FromBody] LoginWithRefreshTokenRequest loginWithRefreshTokenRequest)
    {
        var loginResponse = await Mediator.Send(loginWithRefreshTokenRequest);

        return await AuthorizeUser(loginResponse);
    }

    private async Task<IActionResult> AuthorizeUser(LoginWithCredentialsResponse loginWithCredentialsResponse)
    {
        if (loginWithCredentialsResponse != null)
        {
            var (token, validTo) = AuthenticationConfiguration.CreateToken(loginWithCredentialsResponse, _configuration);

            var refreshToken = await _userService.AddRefreshTokenAsync(
                new RefreshToken
                {
                    Email = loginWithCredentialsResponse.Email, 
                    Refreshtoken = Guid.NewGuid().ToString().ToSha512()
                });

            return Ok(
              new UserApiTokenModel
              {
                  Token = token,
                  Expiration = validTo,
                  RefreshToken = refreshToken.Refreshtoken
              });
        }

        return Unauthorized();
    }
}