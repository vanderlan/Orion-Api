using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VBaseProject.Api.AutoMapper;
using VBaseProject.Api.Models;
using VBaseProject.Controllers;
using VBaseProject.Service.Interfaces;
using static VBaseProject.Service.Authentication.AuthenticationConfiguration;

namespace VBaseProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ApiController
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService, IMapper mapper) : base(mapper)
        {
            _userService = userService;
        }

        [Route("login")]
        [HttpPost]
        public async Task<ActionResult> Login([FromBody] UserLoginModel model)
        {
            var userOutput = _mapper.Map<UserOutput>(await _userService.LoginAsync(model.Email, model.Password));

            if (userOutput != null)
            {
                var claim = new[] {
                    new Claim(JwtRegisteredClaimNames.Email, userOutput.Email),
                    new Claim(JwtRegisteredClaimNames.GivenName, userOutput.FirstName),
                    new Claim(JwtRegisteredClaimNames.UniqueName, userOutput.PublicId),
                    new Claim(ClaimTypes.Role, userOutput.ProfileDescription),
                };

                var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT.SymmetricSecurityKey));

                var token = new JwtSecurityToken(
                  issuer: JWT.Issuer,
                  audience: JWT.Audience,
                  expires: DateTime.UtcNow.AddDays(JWT.TokenExpirationDays),
                  signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256),
                  claims: claim
                );

                return Ok(
                  new
                  {
                      token = new JwtSecurityTokenHandler().WriteToken(token),
                      expiration = token.ValidTo
                  });
            }
            return Unauthorized();
        }
    }
}