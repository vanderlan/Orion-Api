using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Orion.Api.AutoMapper.Output;
using Orion.Api.Jwt;
using Orion.Api.Models;
using Orion.Domain.Extensions;
using Orion.Domain.Interfaces;
using Orion.Entities.Domain;

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
        public async Task<IActionResult> Login([FromBody] UserLoginModel model)
        {
            var userOutput = Mapper.Map<UserOutput>(await _userService.LoginAsync(model.Email, model.Password));

            return await AuthorizeUser(userOutput);
        }

        [Route("RefreshToken")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel refreshTokenModel)
        {
            var userOutput = Mapper.Map<UserOutput>(await _userService.GetUserByRefreshTokenAsync(refreshTokenModel.RefreshToken));

            return await AuthorizeUser(userOutput);
        }

        private async Task<IActionResult> AuthorizeUser(UserOutput userOutput)
        {
            if (userOutput != null)
            {
                var token = CreateToken(userOutput);

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

        private JwtSecurityToken CreateToken(UserOutput userOutput)
        {
            var jwtConfiguration = _configuration.GetSection("JwtConfiguration").Get<JwtConfiguration>();

            var claim = new[] {
                    new Claim(JwtRegisteredClaimNames.Email, userOutput.Email),
                    new Claim(JwtRegisteredClaimNames.GivenName, userOutput.Name),
                    new Claim(JwtRegisteredClaimNames.UniqueName, userOutput.PublicId),
                    new Claim(ClaimTypes.Role, userOutput.ProfileDescription),
                };

            var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SymmetricSecurityKey));

            var token = new JwtSecurityToken(
              issuer: jwtConfiguration.Issuer,
              audience: jwtConfiguration.Audience,
              expires: DateTime.UtcNow.AddMinutes(jwtConfiguration.TokenExpirationMinutes),
              signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256),
              claims: claim
            );

            return token;
        }
    }
}