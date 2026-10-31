using System;
using System.Threading.Tasks;
using Company.Orion.Domain.Core.Repositories;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using Company.Orion.Infra.Data.Mongo.Configuration;
using Company.Orion.Infra.Data.Mongo.Context;
using Company.Orion.Infra.Data.Mongo.Repository.Implementations;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Company.Orion.Infra.Data.Mongo.UnitOfWork;

/// <summary>
/// MongoDb unit of work. Every instance owns its own <see cref="MongoDataContext"/> (the
/// pending writes buffer), while the <see cref="IMongoClient"/> is shared as a singleton
/// to take advantage of the driver connection pool.
/// </summary>
public sealed class MongoUnitOfWork(IMongoClient mongoClient, MongoDbOptions options, ILogger<MongoUnitOfWork> logger) : IUnitOfWork
{
    private MongoDataContext DbContext { get; } = new(mongoClient, options);

    private IUserRepository _userRepository;
    public IUserRepository UserRepository => _userRepository ??= new MongoUserRepository(DbContext);

    private IRefreshTokenRepository _refreshTokenRepository;
    public IRefreshTokenRepository RefreshTokenRepository => _refreshTokenRepository ??= new MongoRefreshTokenRepository(DbContext);

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
            logger.LogError(e, "Error when trying commit changes. Error message: {ErrorMessage}", e.Message);
            DiscardChanges();
            logger.LogWarning("The changes were successfully rolled back");
            throw;
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
