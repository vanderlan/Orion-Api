using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Orion.Data.Context;
using Orion.Data.Repository.Implementations;
using Orion.Domain.Repositories;
using Orion.Domain.Repositories.UnitOfWork;

namespace Orion.Data.UnitOfWork
{
    public class UnitOfWorkEntity : IUnitOfWorkEntity, IDisposable
    {
        private DataContext DbContext { get; }

        public UnitOfWorkEntity(IConfiguration configuration)
        {
            DbContext = new DataContext(GetOptions(configuration.GetSection("DatabaseOptions:ConnectionString").Value));
        }

        public UnitOfWorkEntity(DbContextOptions<DataContext> dbContextOptions)
        {
            DbContext = new DataContext(dbContextOptions);
        }

        private ICustomerRepository _customerRepository;
        public ICustomerRepository CustomerRepository => _customerRepository ??= new CustomerRepository(DbContext);

        private IUserRepository _userRepository;
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(DbContext);

        private IRefreshTokenRepository _refreshTokenRepository;

        public IRefreshTokenRepository RefreshTokenRepository => _refreshTokenRepository ??= new RefreshTokenRepository(DbContext);

        public async Task CommitAsync()
        {
            await DbContext.SaveChangesAsync();
        }

        private static DbContextOptions GetOptions(string connection)
        {
            return new DbContextOptionsBuilder().UseSqlServer(connection).Options;
        }

        public void Dispose()
        {
            DbContext.Dispose();
            GC.SuppressFinalize(this);
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
