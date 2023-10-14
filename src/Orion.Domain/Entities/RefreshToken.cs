namespace Orion.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Refreshtoken { get; set; }
    public string Email { get; set; }
}
