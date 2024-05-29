﻿// <auto-generated />
#if (systemDatabase == SqlServer)
using System;
using Company.Orion.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Company.Orion.Infra.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240215114931_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
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
                b.Property<long>("UserId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("bigint");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("UserId"));

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
                        UserId = 638435945715400715L,
                        CreatedAt = new DateTime(2024, 2, 15, 11, 49, 31, 540, DateTimeKind.Utc).AddTicks(746),
                        Email = "adm@orion.com",
                        LastUpdated = new DateTime(2024, 2, 15, 11, 49, 31, 540, DateTimeKind.Utc).AddTicks(749),
                        Name = "Orion Admin User",
                        Password = "3c9909afec25354d551dae21590bb26e38d53f2173b8d3dc3eee4c047e7ab1c1eb8b85103e3be7ba613b31bb5c9c36214dc9f14a42fd7a2fdb84856bca5c44c2",
                        Profile = 1,
                        PublicId = "874d0ee7-6f77-4857-bac9-7b63b2b4ccaf"
                    });
            });
#pragma warning restore 612, 618
        }
    }
}
#else
using System;
using Company.Orion.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Company.Orion.Infra.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240528122840_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Company.Orion.Domain.Core.Entities.RefreshToken", b =>
                {
                    b.Property<string>("Refreshtoken")
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PublicId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Refreshtoken");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("PublicId")
                        .IsUnique();

                    b.ToTable("RefreshToken", (string)null);
                });

            modelBuilder.Entity("Company.Orion.Domain.Core.Entities.User", b =>
                {
                    b.Property<long>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("UserId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.Property<int>("Profile")
                        .HasColumnType("integer");

                    b.Property<string>("PublicId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("UserId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("PublicId")
                        .IsUnique();

                    b.ToTable("User", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = 638524961200726109L,
                            CreatedAt = new DateTime(2024, 5, 28, 12, 28, 40, 72, DateTimeKind.Utc).AddTicks(6137),
                            Email = "adm@orion.com",
                            LastUpdated = new DateTime(2024, 5, 28, 12, 28, 40, 72, DateTimeKind.Utc).AddTicks(6138),
                            Name = "Orion Admin User",
                            Password = "3c9909afec25354d551dae21590bb26e38d53f2173b8d3dc3eee4c047e7ab1c1eb8b85103e3be7ba613b31bb5c9c36214dc9f14a42fd7a2fdb84856bca5c44c2",
                            Profile = 1,
                            PublicId = "6b6373db-22d2-4faf-b025-1e0c0762dd36"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
#endif
