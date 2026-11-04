using System;

namespace Company.Orion.Test.Shared;

/// <summary>
/// Deterministic <see cref="TimeProvider"/> for unit tests (no dependency on Microsoft.Extensions.TimeProvider.Testing).
/// </summary>
public sealed class TestTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
    private DateTimeOffset _utcNow = utcNow;

    public override DateTimeOffset GetUtcNow() => _utcNow;

    public void Advance(TimeSpan by) => _utcNow = _utcNow.Add(by);
}
