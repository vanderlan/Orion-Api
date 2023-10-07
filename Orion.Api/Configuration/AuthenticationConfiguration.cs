using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Orion.Api.AutoMapper.Output;
using Orion.Api.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Orion.Api.Configuration
{
    public static class AuthenticationConfiguration
    {
        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidIssuer = jwtOptions.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SymmetricSecurityKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        public static JwtSecurityToken CreateToken(UserOutput userOutput, IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();

            var claim = new[] {
                    new Claim(JwtRegisteredClaimNames.Email, userOutput.Email),
                    new Claim(JwtRegisteredClaimNames.GivenName, userOutput.Name),
                    new Claim(JwtRegisteredClaimNames.UniqueName, userOutput.PublicId),
                    new Claim(ClaimTypes.Role, userOutput.ProfileDescription),
                };

            var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SymmetricSecurityKey));

            var token = new JwtSecurityToken(
              issuer: jwtOptions.Issuer,
              audience: jwtOptions.Audience,
              expires: DateTime.UtcNow.AddMinutes(jwtOptions.TokenExpirationMinutes),
              signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha512),
              claims: claim
            );

            return token;
        }
    }
}
