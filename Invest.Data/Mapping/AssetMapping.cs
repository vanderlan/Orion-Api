using Invest.Entities.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invest.Data.Mapping
{
    public class AssetMapping : IEntityTypeConfiguration<Asset>
    {
        public void Configure(EntityTypeBuilder<Asset> builder)
        {
            builder.Property(x => x.Code).HasMaxLength(100).IsRequired();
            builder.Property(x => x.PublicId).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(200).IsRequired();
            builder.HasIndex(x => x.Code).IsUnique();
        }
    }
}
