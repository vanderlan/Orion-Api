namespace Orion.Entities.Domain
{
    public class RefreshToken : BaseEntity
    {
        public string Refreshtoken { get; set; }
        public string Email { get; set; }
    }
}
