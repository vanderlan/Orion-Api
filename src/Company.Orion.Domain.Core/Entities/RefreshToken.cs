namespace Company.Orion.Domain.Core.Entities;

public class RefreshToken : BaseEntity
{
    public required string Refreshtoken { get; init; }
    public required string Email { get; init; }

    /// <summary>
    /// A refresh token is valid for a fixed lifetime counted from its creation. Rotation always
    /// issues a brand new token, so the lifetime restarts on every successful refresh.
    /// </summary>
    public bool IsExpired(DateTime utcNow, TimeSpan lifetime)
    {
        return CreatedAt.Add(lifetime) <= utcNow;
    }
}
