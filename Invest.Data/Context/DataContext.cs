using Invest.Data.Mapping;
using Invest.Entities.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Invest.Data.Context
{
    public class DataContext : DbContext
    {
        public DataContext() : base(GetOptions())
        {

        }
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        #region DBSet
        public DbSet<Asset> Assets { get; set; }
        public DbSet<User> Users { get; set; }
        #endregion

        private static DbContextOptions GetOptions()
        {
            var connectionString = "Server=tcp:investdbmyinv.cuevoikrp8by.us-east-1.rds.amazonaws.com,1433;Initial Catalog=MyInv;Persist Security Info=False;User ID=admin;Password=123Ab321;MultipleActiveResultSets=False;TrustServerCertificate=False;Connection Timeout=30;";

            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AssetMapping());
            modelBuilder.ApplyConfiguration(new UserMapping());

            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                entityType.Relational().TableName = entityType.DisplayName();
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
                    ((BaseEntity)entity.Entity).CreatedAt = DateTime.Now;
                    ((BaseEntity)entity.Entity).PublicId = Guid.NewGuid().ToString();
                }

                ((BaseEntity)entity.Entity).LastUpdated = DateTime.Now;
            }
        }
    }
}
