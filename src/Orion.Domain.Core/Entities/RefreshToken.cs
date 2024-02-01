namespace Orion.Domain.Core.Entities;

public class RefreshToken : BaseEntity
{
    public string Refreshtoken { get; init; }
    public string Email { get; init; }
}
