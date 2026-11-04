using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Company.Orion.Domain.Core.Repositories;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using Company.Orion.Infra.Data.Context;
using Company.Orion.Infra.Data.Repository.Implementations;

namespace Company.Orion.Infra.Data.UnitOfWork;

/// <summary>
/// Scoped unit of work over the <see cref="DataContext"/> owned by the DI scope (one context per
/// request, shared by every repository resolved in the same scope).
/// </summary>
public sealed class UnitOfWork(DataContext dbContext, ILogger<UnitOfWork> logger) : IUnitOfWork
{
    private DataContext DbContext { get; } = dbContext;

    private IUserRepository _userRepository;
    public IUserRepository UserRepository => _userRepository ??= new UserRepository(DbContext);

    private IRefreshTokenRepository _refreshTokenRepository;
    public IRefreshTokenRepository RefreshTokenRepository => _refreshTokenRepository ??= new RefreshTokenRepository(DbContext);

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Trying to commit changes");

            await DbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("The changes were successfully committed");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error when trying commit changes. Error message: {ErrorMessage}", e.Message);
            DiscardChanges();
            logger.LogWarning("The changes were successfully rolled back");
            throw;
        }
    }

    public async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        //Joins the ambient transaction when one was already opened by an outer operation.
        if (DbContext.Database.CurrentTransaction is not null)
        {
            await operation();
            return;
        }

        await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            logger.LogInformation("Trying to execute the operation in a transaction");

            await operation();
            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation("The transaction was successfully committed");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error when trying to execute the transactional operation. Error message: {ErrorMessage}", e.Message);
            await transaction.RollbackAsync(CancellationToken.None);
            DiscardChanges();
            logger.LogWarning("The transaction was successfully rolled back");
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

    /// <summary>The context is owned and disposed by the DI scope; nothing to release here.</summary>
    public void Dispose()
    {
    }
}
