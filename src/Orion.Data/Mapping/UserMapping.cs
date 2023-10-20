using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Orion.Domain.Entities.Enuns;
using Orion.Domain.Entities;
using Orion.Domain.Extensions;

namespace Orion.Data.Mapping;

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
                UserId = 1,
                PublicId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                Email = "adm@orion-api.com",
                Name = "Orion Admin User",
                LastUpdated = DateTime.UtcNow,
                Profile = UserProfile.Admin,
                Password =  "123".ToSha512()
            }
        );
    }
}
