using Microsoft.EntityFrameworkCore;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Infra.Data.Mapping;

namespace Company.Orion.Infra.Data.Context;

/// <summary>
/// EF Core context. Configured by the host through <c>AddDbContextPool</c> (provider, connection
/// string and interceptors), so it has no knowledge of IConfiguration. Timestamps and public ids
/// are assigned by <see cref="Interceptors.TimestampsInterceptor"/>.
/// </summary>
public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    #region DBSet
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserMapping());
        modelBuilder.ApplyConfiguration(new RefreshTokenMapping());

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            modelBuilder.Entity(entityType.ClrType).ToTable(entityType.ClrType.Name);
        }
    }
}
