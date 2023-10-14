using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Orion.Data.Context;
using Orion.Data.Repository.Implementations;
using Orion.Domain.Repositories;
using Orion.Domain.Repositories.UnitOfWork;

namespace Orion.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private DataContext DbContext { get; }

    public UnitOfWork(IConfiguration configuration)
    {
        DbContext = new DataContext(GetOptions(configuration.GetSection("DatabaseOptions:ConnectionString").Value));
    }

    public UnitOfWork(DbContextOptions<DataContext> dbContextOptions)
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
        return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connection).Options;
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

    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
