using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Company.Orion.Domain.Core.Repositories;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using Company.Orion.Infra.Data.Context;
using Company.Orion.Infra.Data.Repository.Implementations;

namespace Company.Orion.Infra.Data.UnitOfWork;

public sealed class UnitOfWork(IConfiguration configuration, ILogger<UserRepository> logger) : IUnitOfWork
{
    private DataContext DbContext { get; } = new(configuration);

    private IUserRepository _userRepository;
    public IUserRepository UserRepository => _userRepository ??= new UserRepository(DbContext);

    private IRefreshTokenRepository _refreshTokenRepository;
    public IRefreshTokenRepository RefreshTokenRepository => _refreshTokenRepository ??= new RefreshTokenRepository(DbContext);

    public async Task CommitAsync()
    {
        try
        {
            logger.LogInformation("Trying to commit changes");
            
            await DbContext.SaveChangesAsync();
            
            logger.LogInformation("The changes were successfully committed");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error when trying commit changes. Error message: {errorMessage}", e.Message);
            DiscardChanges();
            logger.LogWarning("The changes were successfully rolled back");
            throw;
        }
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

    private void Dispose(bool disposing)
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
