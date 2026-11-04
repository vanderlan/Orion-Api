using Company.Orion.Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Company.Orion.Infra.Data.Interceptors;

/// <summary>
/// Assigns <see cref="BaseEntity.PublicId"/> and <see cref="BaseEntity.CreatedAt"/> to new entities
/// and refreshes <see cref="BaseEntity.LastUpdated"/> on every save. Registered once (singleton)
/// on the context options, which keeps <see cref="Context.DataContext"/> poolable and the clock testable.
/// </summary>
public sealed class TimestampsInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        AddTimestamps(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        AddTimestamps(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void AddTimestamps(DbContext context)
    {
        if (context is null)
            return;

        var utcNow = timeProvider.GetUtcNow().UtcDateTime;

        var entries = context.ChangeTracker.Entries<BaseEntity>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
                entry.Entity.PublicId = Guid.NewGuid().ToString();
            }

            entry.Entity.LastUpdated = utcNow;
        }
    }
}
