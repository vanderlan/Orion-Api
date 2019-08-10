using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VBaseProject.Entities.Domain;

namespace VBaseProject.Data.Mapping
{
    public class CustomerMapping : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.PublicId).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Address).HasMaxLength(200).IsRequired();
            builder.Property(x => x.PhoneNumber).HasMaxLength(20).IsRequired();
            builder.HasIndex(x => x.Name).IsUnique();
        }
    }
}
