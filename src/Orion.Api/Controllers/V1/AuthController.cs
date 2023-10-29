using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orion.Api.AutoMapper.Output;
using Orion.Api.Configuration;
using Orion.Api.Controllers.Base;
using Orion.Api.Models;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Extensions;
using Orion.Domain.Core.Services.Interfaces;

namespace Orion.Api.Controllers.V1;

[ApiVersion(1.0)]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ApiController
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public AuthController(IUserService userService, IMapper mapper, IConfiguration configuration) : base(mapper)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [Route("Login")]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] UserLoginModel model)
    {
        var userOutput = Mapper.Map<UserOutput>(await _userService.LoginAsync(model.Email, model.Password));

        return await AuthorizeUser(userOutput);
    }

    [Route("RefreshToken")]
    [HttpPost]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel refreshTokenModel)
    {
        var userOutput = Mapper.Map<UserOutput>(await _userService.SignInWithRehreshTokenAsync(refreshTokenModel.RefreshToken, refreshTokenModel.Token));

        return await AuthorizeUser(userOutput);
    }

    private async Task<IActionResult> AuthorizeUser(UserOutput userOutput)
    {
        if (userOutput != null)
        {
            var (Token, ValidTo) = AuthenticationConfiguration.CreateToken(userOutput, _configuration);

            var refreshToken = await _userService.AddRefreshTokenAsync(new RefreshToken { Email = userOutput.Email, Refreshtoken = Guid.NewGuid().ToString().ToSha512() });

            return Ok(
              new UserApiTokenModel
              {
                  Token = Token,
                  Expiration = ValidTo,
                  RefreshToken = refreshToken.Refreshtoken
              });
        }

        return Unauthorized();
    }
}