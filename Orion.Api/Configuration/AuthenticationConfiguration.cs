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
            var jwtConfiguration = configuration.GetSection("JwtConfiguration").Get<JwtSettings>();

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
                    ValidAudience = jwtConfiguration.Audience,
                    ValidIssuer = jwtConfiguration.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SymmetricSecurityKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        public static JwtSecurityToken CreateToken(UserOutput userOutput, IConfiguration configuration)
        {
            var jwtConfiguration = configuration.GetSection("JwtConfiguration").Get<JwtSettings>();

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
