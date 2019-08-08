using Invest.Data.Context;
using Invest.Data.Repository.Implementations;
using Invest.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace Invest.Data.UnitOfWork
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
        private IAssetRepository _assetRepository;
        public IAssetRepository AssetRepository => _assetRepository ?? (_assetRepository = new AssetRepository(DbContext));

        private IUserRepository _userRepository;
        public IUserRepository UserRepository => _userRepository ?? (_userRepository = new UserRepository(DbContext));

        public async Task CommitAsync()
        {
            await DbContext.SaveChangesAsync();
        }

        private static DbContextOptions GetOptions(string connection)
        {
            // var connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=MyInv;Integrated Security=True;MultipleActiveResultSets=True;";
            //var connectionString = "Server=tcp:myinv-server.database.windows.net,1433;Initial Catalog=MyinvDev;Persist Security Info=False;User ID=myinadmin;Password=123Ab321;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

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
