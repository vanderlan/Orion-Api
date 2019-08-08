using System;

namespace Invest.Entities.Domain
{
    public abstract class BaseEntity
    {
        public string PublicId { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}