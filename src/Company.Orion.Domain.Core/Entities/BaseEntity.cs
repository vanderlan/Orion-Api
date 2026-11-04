namespace Company.Orion.Domain.Core.Entities;

public abstract class BaseEntity
{
    /// <summary>Public identifier (GUID) assigned by the data layer when the entity is first persisted.</summary>
    public string PublicId { get; set; } = string.Empty;

    /// <summary>Filled by the data layer on every commit (UTC).</summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>Filled by the data layer when the entity is first persisted (UTC).</summary>
    public DateTime CreatedAt { get; set; }
}
