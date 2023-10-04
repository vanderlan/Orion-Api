namespace Orion.Api.Jwt
{
    public class JwtSettings
    {
        public string SymmetricSecurityKey { get; set; } 
        public string Issuer { get; set; }
        public string Audience { get; set; } 
        public int TokenExpirationMinutes { get; set; }
    }
}
