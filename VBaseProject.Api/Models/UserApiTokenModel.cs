using System;

namespace VBaseProject.Api.Models
{
    public class UserApiTokenModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}
