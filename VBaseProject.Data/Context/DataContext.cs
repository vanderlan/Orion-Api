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
            var prodConn = "Server=tcp:vbaseproject-api.ddns.net,1433;Initial Catalog=vbaseprojectapi;Persist Security Info=False;User ID=sa;Password=123Ab321;MultipleActiveResultSets=False;Connection Timeout=30;";
            var connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=VBaseProject;Integrated Security=True;MultipleActiveResultSets=True;";

            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), prodConn).Options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerMapping());
            modelBuilder.ApplyConfiguration(new UserMapping());

            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                //entityType.Relational().TableName = entityType.DisplayName();
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
