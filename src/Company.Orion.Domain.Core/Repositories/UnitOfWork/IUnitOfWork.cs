namespace Company.Orion.Domain.Core.Repositories.UnitOfWork;

/// <summary>
/// Scoped (one per request) entry point to the persistence. Repositories created by the same
/// unit of work share the same context, so every write is persisted by a single <see cref="CommitAsync"/>.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IUserRepository UserRepository { get; }
    IRefreshTokenRepository RefreshTokenRepository { get; }

    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the given operation inside a database transaction, guaranteeing that all the
    /// writes performed by the operation are persisted atomically: either every change is
    /// committed or every change is rolled back.
    /// </summary>
    /// <param name="operation">The operation to be executed. It is expected to stage the writes through the repositories and call <see cref="CommitAsync"/>.</param>
    /// <param name="cancellationToken">Propagated to the transaction start/commit/rollback.</param>
    Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);

    void DiscardChanges();
}
