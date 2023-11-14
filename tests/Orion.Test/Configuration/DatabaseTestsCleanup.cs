using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Repositories.UnitOfWork;

namespace Orion.Test.Configuration
{
    public static class DatabaseTestsCleanup
    {
        public static void BeforeEachTest(IConfiguration configuration, IUnitOfWork unitOfWork, User defaultSystemUser)
        {
            lock (unitOfWork)
            {
                var tablesToTruncate = new[] { "User", "RefreshToken" };

                using SqlConnection connection = new(configuration["ConnectionStrings:OrionDatabase"]);
                connection.Open();

                foreach (var table in tablesToTruncate)
                {
                    var command = new SqlCommand($"TRUNCATE TABLE dbo.[{table}]", connection);

                    command.ExecuteNonQuery();
                }

                unitOfWork.UserRepository.AddAsync(defaultSystemUser).GetAwaiter().GetResult();
                unitOfWork.CommitAsync().GetAwaiter().GetResult();
            }
        }
    }
}
