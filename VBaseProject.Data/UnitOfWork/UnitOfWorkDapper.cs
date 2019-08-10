using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using VBaseProject.Data.Context;

namespace VBaseProject.Data.UnitOfWork
{
    public class UnitOfWorkDapper : IUnitOfWorkDapper
    {
        private readonly IDbConnection _connection;

        public UnitOfWorkDapper(IOptions<DatabaseOptions> databaseOptions)
        {
            _connection = new SqlConnection(databaseOptions.Value.ConnectionString);
        }
        public UnitOfWorkDapper(string connection)
        {
            _connection = new SqlConnection(connection);
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
