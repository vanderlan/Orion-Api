using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Linq;
using System.Threading.Tasks;
using VBaseProject.Data.Mapping;
using VBaseProject.Entities.Domain;

namespace VBaseProject.Data.Context
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
        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        #endregion

        private static DbContextOptions GetOptions()
        {
            var connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=VBaseProject;Integrated Security=True;MultipleActiveResultSets=True;";

            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerMapping());
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
