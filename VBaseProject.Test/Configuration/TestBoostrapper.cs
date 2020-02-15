using Microsoft.EntityFrameworkCore;
using VBaseProject.Data.Context;

namespace VBaseProject.Test.Configuration
{
    public class TestBootstrapper
    {
        /// <summary>
        /// Create an instance of in memory database context for testing.
        /// Use the returned DbContextOptions to initialize DbContext.
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static DbContextOptions<DataContext> GetInMemoryDbContextOptions(string dbName = "Test_DB")
        {
            var context = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .EnableDetailedErrors(true);

            return context.Options;
        }
    }
}
