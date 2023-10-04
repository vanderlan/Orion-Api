using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orion.Api.AutoMapper.Output;
using Orion.Api.Configuration;
using Orion.Api.Models;
using Orion.Domain.Extensions;
using Orion.Domain.Interfaces;
using Orion.Entities.Domain;
using System.IdentityModel.Tokens.Jwt;

namespace Orion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginModel userLoginModel)
        {
            var userOutput = Mapper.Map<UserOutput>(await _userService.LoginAsync(userLoginModel.Email, userLoginModel.Password));

            return await AuthorizeUserAsync(userOutput);
        }

        [Route("RefreshToken")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel refreshTokenModel)
        {
            var userOutput = Mapper.Map<UserOutput>(await _userService.GetUserByRefreshTokenAsync(refreshTokenModel.RefreshToken));

            return await AuthorizeUserAsync(userOutput);
        }

        private async Task<IActionResult> AuthorizeUserAsync(UserOutput userOutput)
        {
            if (userOutput != null)
            {
                var token = AuthenticationConfiguration.CreateToken(userOutput, _configuration);

                var refreshToken = await _userService.AddRefreshTokenAsync(new RefreshToken { Email = userOutput.Email, Refreshtoken = Guid.NewGuid().ToString().ToSha512()});

                return Ok(
                  new UserApiTokenModel
                  {
                      Token = new JwtSecurityTokenHandler().WriteToken(token),
                      Expiration = token.ValidTo,
                      RefreshToken = refreshToken.Refreshtoken
                  });
            }

            return Unauthorized();
        }
    }
}