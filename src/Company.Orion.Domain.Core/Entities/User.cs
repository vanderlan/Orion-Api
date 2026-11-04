using Company.Orion.Domain.Core.Entities.Enuns;

namespace Company.Orion.Domain.Core.Entities;

public class User : BaseEntity
{
    public long UserId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required UserProfile Profile { get; set; }

    /// <summary>Password hash produced by <c>IPasswordHasher</c>, never the plain text password.</summary>
    public string Password { get; set; } = string.Empty;
}
