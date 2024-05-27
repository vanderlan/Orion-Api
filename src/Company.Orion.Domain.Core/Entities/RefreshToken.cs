namespace Company.Orion.Domain.Core.Entities;

public class RefreshToken : BaseEntity
{
    public required string Refreshtoken { get; init; }
    public required string Email { get; init; }
}
