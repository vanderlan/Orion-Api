using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Orion.Domain.Core.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Orion.Infra.Data.Mapping;

namespace Orion.Infra.Data.Context;

public class DataContext : DbContext
{
    public ModelBuilder ModelBuilder { get; private set; }

    public DataContext(IConfiguration configuration) : base(GetDefaultOptions(configuration))
    {

    }

    public DataContext(DbContextOptions options) : base(options)
    {

    }

    #region DBSet
    public DbSet<Customer> Customers { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    #endregion

    private static DbContextOptions GetDefaultOptions(IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("DatabaseOptions:ConnectionString").Value;

        return new DbContextOptionsBuilder().UseSqlServer(connectionString).Options;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerMapping());
        modelBuilder.ApplyConfiguration(new UserMapping());
        modelBuilder.ApplyConfiguration(new RefreshTokenMapping());

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            modelBuilder.Entity(entityType.ClrType).ToTable(entityType.ClrType.Name);
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        AddTimestamps();
        return await base.SaveChangesAsync();
    }

    private void AddTimestamps()
    {
        var entities = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entity in entities)
        {
            if (entity.State == EntityState.Added)
            {
                ((BaseEntity)entity.Entity).CreatedAt = DateTime.UtcNow;
                ((BaseEntity)entity.Entity).PublicId = Guid.NewGuid().ToString();
            }

            ((BaseEntity)entity.Entity).LastUpdated = DateTime.UtcNow;
        }
    }
}
