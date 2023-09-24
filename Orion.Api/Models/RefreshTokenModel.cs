using System.ComponentModel.DataAnnotations;

namespace Orion.Api.Models
{
    public class RefreshTokenModel
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
