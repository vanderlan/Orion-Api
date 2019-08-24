using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using VBaseProject.Data.Context;
using VBaseProject.Data.Repository.Implementations;
using VBaseProject.Data.Repository.Interfaces;

namespace VBaseProject.Data.UnitOfWork
{
    public class UnitOfWorkEntity : IUnitOfWorkEntity
    {
        private DataContext DbContext { get; }

        public UnitOfWorkEntity(IOptions<DatabaseOptions> databaseOptions)
        {
            DbContext = new DataContext(GetOptions(databaseOptions.Value.ConnectionString));
        }
        public UnitOfWorkEntity(string connection)
        {
            DbContext = new DataContext(GetOptions(connection));
        }

        private ICustomerRepository _assetRepository;
        public ICustomerRepository CustomerRepository => _assetRepository ?? (_assetRepository = new CustomerRepository(DbContext));

        private IUserRepository _userRepository;
        public IUserRepository UserRepository => _userRepository ?? (_userRepository = new UserRepository(DbContext));

        private IRefreshTokenRepository _refreshTokenRepository;
        public IRefreshTokenRepository RefreshTokenRepository => _refreshTokenRepository ?? (_refreshTokenRepository = new RefreshTokenRepository(DbContext));

        public async Task CommitAsync()
        {
            await DbContext.SaveChangesAsync();
        }

        private static DbContextOptions GetOptions(string connection)
        {
            //var connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=MyInv;Integrated Security=True;MultipleActiveResultSets=True;";

            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connection).Options;
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }

        public void DiscardChanges()
        {
            foreach (var entry in DbContext.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged))
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                }
            }
        }
    }
}
