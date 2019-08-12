namespace VBaseProject.Entities.Domain
{
    public class RefreshToken : BaseEntity
    {
        public int RefreshTokenId { get; set; }
        public string Refreshtoken { get; set; }
        public string Email { get; set; }
    }
}
