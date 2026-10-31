using System;
using System.Threading;
#if (systemDatabase == SqlServer)
using Testcontainers.MsSql;
#elif (systemDatabase == PostgreSql)
using Testcontainers.PostgreSql;
#else
using Testcontainers.MongoDb;
#endif

namespace Company.Orion.Test.Integration.Setup;

/// <summary>
/// Starts a single throwaway database container for the whole test run.
/// The container is removed automatically when the test process exits.
/// </summary>
public static class TestDatabaseContainer
{
    private static readonly Lazy<string> LazyConnectionString =
        new(StartContainer, LazyThreadSafetyMode.ExecutionAndPublication);

    public static string ConnectionString => LazyConnectionString.Value;

#if (systemDatabase == SqlServer)
    private static string StartContainer()
    {
        var container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2019-latest").Build();

        container.StartAsync().GetAwaiter().GetResult();

        return container.GetConnectionString();
    }
#elif (systemDatabase == PostgreSql)
    private static string StartContainer()
    {
        var container = new PostgreSqlBuilder("postgres:16.3").Build();

        container.StartAsync().GetAwaiter().GetResult();

        return container.GetConnectionString();
    }
#else
    private static string StartContainer()
    {
        var container = new MongoDbBuilder("mongo:7.0").Build();

        container.StartAsync().GetAwaiter().GetResult();

        return container.GetConnectionString();
    }
#endif
}
