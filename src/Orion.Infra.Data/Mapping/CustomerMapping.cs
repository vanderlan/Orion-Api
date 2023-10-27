using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orion.Core.Domain.Entities;

namespace Orion.Infra.Data.Mapping;

public class CustomerMapping : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.PublicId).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasIndex(x => x.PublicId).IsUnique();
    }
}
