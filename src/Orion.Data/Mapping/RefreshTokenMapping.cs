using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orion.Entities.Domain;

namespace Orion.Data.Mapping
{
    public class RefreshTokenMapping : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.Property(x => x.PublicId).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Refreshtoken).HasMaxLength(300).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(200).IsRequired();
            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasKey(x => x.Refreshtoken);
        }
    }
}
