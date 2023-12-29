using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Orion.Domain.Core.Repositories;
using Orion.Domain.Core.Repositories.UnitOfWork;
using Orion.Infra.Data.Context;
using Orion.Infra.Data.Repository.Implementations;

namespace Orion.Infra.Data.UnitOfWork;

public class UnitOfWork(IConfiguration configuration) : IUnitOfWork
{
    private DataContext DbContext { get; } = new(configuration);

    private IUserRepository _userRepository;
    public IUserRepository UserRepository => _userRepository ??= new UserRepository(DbContext);

    private IRefreshTokenRepository _refreshTokenRepository;
    public IRefreshTokenRepository RefreshTokenRepository => _refreshTokenRepository ??= new RefreshTokenRepository(DbContext);

    public async Task CommitAsync()
    {
        await DbContext.SaveChangesAsync();
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
