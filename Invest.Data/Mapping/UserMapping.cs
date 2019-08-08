using Invest.Entities.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invest.Data.Mapping
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.PublicId).HasMaxLength(50).IsRequired();
            builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(200).IsRequired();
            builder.HasIndex(x => x.Email).IsUnique();
            builder.Property(x => x.Password).HasMaxLength(300).IsRequired();
        }
    }
}
