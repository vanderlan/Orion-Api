using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Company.Orion.Domain.Core.Entities.Enuns;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Extensions;
using System.Globalization;

namespace Company.Orion.Infra.Data.Mapping;

public class UserMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.PublicId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.PublicId).IsUnique();
        builder.Property(x => x.Password).HasMaxLength(300).IsRequired();

        //Default System User
        builder.HasData(
            new User
            {
                UserId = 923165498765432123,
                PublicId = "16537902-1ca7-49ca-82e5-0be137f9aeeb",
                CreatedAt = DateTime.Parse("2025-01-01", new CultureInfo("en-US")),
                Email = "adm@orion.com",
                Name = "Orion Admin User",
                LastUpdated = DateTime.Parse("2025-01-01", new CultureInfo("en-US")),
                Profile = UserProfile.Admin,
                Password =  "123".ToSha512()
            }
        );
    }
}
