namespace VBaseProject.Api.Jwt
{
    public class JwtConfiguration
    {
        public string SymmetricSecurityKey { get; set; } 
        public string Issuer { get; set; }
        public string Audience { get; set; } 
        public int TokenExpirationMinutes { get; set; }
    }
}
