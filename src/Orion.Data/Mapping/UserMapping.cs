using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Orion.Domain.Entities.Enuns;
using Orion.Domain.Entities;

namespace Orion.Data.Mapping;

public class UserMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.PublicId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.Password).HasMaxLength(300).IsRequired();

        //Default System User
        builder.HasData(
            new User
            {
                UserId = 1,
                PublicId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.Now,
                Email = "vanderlan.gs@gmail.com",
                Name = "Vanderlan Gomes da Silva",
                LastUpdated = DateTime.Now,
                Profile = UserProfile.Admin,
                Password = "3c9909afec25354d551dae21590bb26e38d53f2173b8d3dc3eee4c047e7ab1c1eb8b85103e3be7ba613b31bb5c9c36214dc9f14a42fd7a2fdb84856bca5c44c2"
            }
        );
    }
}
