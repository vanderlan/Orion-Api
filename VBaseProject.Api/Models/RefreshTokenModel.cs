using System.ComponentModel.DataAnnotations;

namespace VBaseProject.Api.Models
{
    public class RefreshTokenModel
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
