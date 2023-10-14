// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Orion.Data.Context;

namespace Orion.Data.Migrations;

[DbContext(typeof(DataContext))]
[Migration("20210313163754_ChangeRefreshTokenSize")]
partial class ChangeRefreshTokenSize
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("Relational:MaxIdentifierLength", 128)
            .HasAnnotation("ProductVersion", "5.0.3")
            .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

        modelBuilder.Entity("Orion.Domain.Entities.Domain.Customer", b =>
            {
                b.Property<int>("CustomerId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int")
                    .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<DateTime>("LastUpdated")
                    .HasColumnType("datetime2");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("nvarchar(100)");

                b.Property<string>("PublicId")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar(50)");

                b.HasKey("CustomerId");

                b.HasIndex("Name")
                    .IsUnique();

                b.ToTable("Customer");
            });

        modelBuilder.Entity("Orion.Domain.Entities.RefreshToken", b =>
            {
                b.Property<string>("Refreshtoken")
                    .HasMaxLength(300)
                    .HasColumnType("nvarchar(300)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("Email")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("nvarchar(200)");

                b.Property<DateTime>("LastUpdated")
                    .HasColumnType("datetime2");

                b.Property<string>("PublicId")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar(50)");

                b.HasKey("Refreshtoken");

                b.HasIndex("Email")
                    .IsUnique();

                b.ToTable("RefreshToken");
            });

        modelBuilder.Entity("Orion.Domain.Entities.User", b =>
            {
                b.Property<int>("UserId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int")
                    .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("Email")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("nvarchar(200)");

                b.Property<DateTime>("LastUpdated")
                    .HasColumnType("datetime2");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("nvarchar(100)");

                b.Property<string>("Password")
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnType("nvarchar(300)");

                b.Property<int>("Profile")
                    .HasColumnType("int");

                b.Property<string>("PublicId")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar(50)");

                b.HasKey("UserId");

                b.HasIndex("Email")
                    .IsUnique();

                b.ToTable("User");

                b.HasData(
                    new
                    {
                        UserId = 1,
                        CreatedAt = new DateTime(2021, 3, 13, 13, 37, 53, 742, DateTimeKind.Local).AddTicks(9943),
                        Email = "vanderlan.gs@gmail.com",
                        LastUpdated = new DateTime(2021, 3, 13, 13, 37, 53, 744, DateTimeKind.Local).AddTicks(5149),
                        Name = "Vanderlan Gomes da Silva",
                        Password = "3c9909afec25354d551dae21590bb26e38d53f2173b8d3dc3eee4c047e7ab1c1eb8b85103e3be7ba613b31bb5c9c36214dc9f14a42fd7a2fdb84856bca5c44c2",
                        Profile = 0,
                        PublicId = "9f2e9852-5d9e-469b-9508-c5532649ae5d"
                    });
            });
#pragma warning restore 612, 618
    }
}
