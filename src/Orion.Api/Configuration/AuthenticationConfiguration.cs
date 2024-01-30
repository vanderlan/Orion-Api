using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Orion.Application.Core.Commands.LoginWithCredentials;
using Orion.Domain.Core.Extensions;

namespace Orion.Api.Configuration;

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
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha512 },
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SymmetricSecurityKey)),
                ClockSkew = TimeSpan.Zero
            };
        });
    }

    public static (string Token, DateTime ValidTo)CreateToken(LoginWithCredentialsResponse loginWithCredentialsResponse, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, loginWithCredentialsResponse.Email),
            new Claim(ClaimTypes.GivenName, loginWithCredentialsResponse.Name),
            new Claim(ClaimTypes.Sid, loginWithCredentialsResponse.PublicId),
            new Claim(ClaimTypes.Role, loginWithCredentialsResponse.Profile.Description()),
        };

        var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SymmetricSecurityKey));

        var token = new JwtSecurityToken(
          issuer: jwtOptions.Issuer,
          audience: jwtOptions.Audience,
          expires: DateTime.UtcNow.AddMinutes(jwtOptions.TokenExpirationMinutes),
          signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha512),
          claims: claims
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo);
    }
}

/// <summary>
/// Section of appsettings that contains the JWT app configuration
/// </summary>
public class JwtOptions
{
    public string SymmetricSecurityKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int TokenExpirationMinutes { get; set; }
}
