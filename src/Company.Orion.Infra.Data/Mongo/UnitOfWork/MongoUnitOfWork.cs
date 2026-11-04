using System;
using System.Threading;
using System.Threading.Tasks;
using Company.Orion.Domain.Core.Repositories;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using Company.Orion.Infra.Data.Mongo.Context;
using Company.Orion.Infra.Data.Mongo.Repository.Implementations;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Company.Orion.Infra.Data.Mongo.UnitOfWork;

/// <summary>
/// Scoped MongoDb unit of work over the <see cref="MongoDataContext"/> (the pending writes buffer)
/// owned by the DI scope, while the <see cref="IMongoClient"/> is shared as a singleton to take
/// advantage of the driver connection pool.
/// </summary>
public sealed class MongoUnitOfWork(MongoDataContext dataContext, ILogger<MongoUnitOfWork> logger) : IUnitOfWork
{
    private MongoDataContext DbContext { get; } = dataContext;

    private IUserRepository _userRepository;
    public IUserRepository UserRepository => _userRepository ??= new MongoUserRepository(DbContext);

    private IRefreshTokenRepository _refreshTokenRepository;
    public IRefreshTokenRepository RefreshTokenRepository => _refreshTokenRepository ??= new MongoRefreshTokenRepository(DbContext);

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

        using var session = await DbContext.Client.StartSessionAsync(cancellationToken: cancellationToken);

        if (!TryStartTransaction(session))
        {
            //Transactions are not supported by the deployment (e.g. standalone servers).
            //The operation is still executed so all its writes are persisted by a single commit.
            await operation();
            return;
        }

        DbContext.UseSession(session);

        try
        {
            logger.LogInformation("Trying to execute the operation in a MongoDb transaction");

            await operation();
            await session.CommitTransactionAsync(cancellationToken);

            logger.LogInformation("The MongoDb transaction was successfully committed");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error when trying to execute the transactional operation. Error message: {ErrorMessage}", e.Message);
            await session.AbortTransactionAsync(CancellationToken.None);
            DiscardChanges();
            logger.LogWarning("The MongoDb transaction was successfully aborted");
            throw;
        }
        finally
        {
            DbContext.UseSession(null);
        }
    }

    private bool TryStartTransaction(IClientSessionHandle session)
    {
        try
        {
            session.StartTransaction();
            return true;
        }
        catch (NotSupportedException e)
        {
            logger.LogWarning(e, "The MongoDb deployment does not support transactions, the operation will be executed without one");
            return false;
        }
    }

    public void DiscardChanges()
    {
        DbContext.DiscardChanges();
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
