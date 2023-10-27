using System;

namespace Orion.Core.Domain.Entities;

public abstract class BaseEntity
{
    public string PublicId { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime CreatedAt { get; set; }
}