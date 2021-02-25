using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq;
using System.Threading.Tasks;
using VBaseProject.Data.Mapping;
using VBaseProject.Entities.Domain;

namespace VBaseProject.Data.Context
{
    public class DataContext : DbContext
    {
        public ModelBuilder ModelBuilder { get; private set; }

        public DataContext() : base(GetOptions())
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

        private static DbContextOptions GetOptions()
        {
            var connectionString = "Server=tcp:191.240.163.171,1433;Initial Catalog=VBaseProject;Persist Security Info=False;User ID=sa;Password=123456Ab;MultipleActiveResultSets=False;Connection Timeout=30;";

            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerMapping());
            modelBuilder.ApplyConfiguration(new UserMapping());
            modelBuilder.ApplyConfiguration(new RefreshTokenMapping());

            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
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
                    ((BaseEntity)entity.Entity).CreatedAt = DateTime.Now;
                    ((BaseEntity)entity.Entity).PublicId = Guid.NewGuid().ToString();
                }

                ((BaseEntity)entity.Entity).LastUpdated = DateTime.Now;
            }
        }
    }
}
