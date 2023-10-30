﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Orion.Infra.Data.Context;

#nullable disable

namespace Orion.Infra.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20231030205851_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Orion.Domain.Core.Entities.RefreshToken", b =>
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

                    b.HasIndex("PublicId")
                        .IsUnique();

                    b.ToTable("RefreshToken", (string)null);
                });

            modelBuilder.Entity("Orion.Domain.Core.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

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

                    b.HasIndex("PublicId")
                        .IsUnique();

                    b.ToTable("User", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            CreatedAt = new DateTime(2023, 10, 30, 20, 58, 51, 289, DateTimeKind.Utc).AddTicks(1590),
                            Email = "adm@orion-api.com",
                            LastUpdated = new DateTime(2023, 10, 30, 20, 58, 51, 289, DateTimeKind.Utc).AddTicks(1592),
                            Name = "Orion Admin User",
                            Password = "3c9909afec25354d551dae21590bb26e38d53f2173b8d3dc3eee4c047e7ab1c1eb8b85103e3be7ba613b31bb5c9c36214dc9f14a42fd7a2fdb84856bca5c44c2",
                            Profile = 1,
                            PublicId = "80302277-52d6-4c78-b7a7-0d458ecdea8b"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
