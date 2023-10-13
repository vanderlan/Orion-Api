using Microsoft.EntityFrameworkCore;
using Orion.Data.Context;

namespace Orion.Test.Configuration
{
    public class TestBootstrapper
    {
        public static DbContextOptions<DataContext> GetInMemoryDbContextOptions(string dbName = "Test_DB")
        {
            var context = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .EnableDetailedErrors(true);

            return context.Options;
        }
    }
}
