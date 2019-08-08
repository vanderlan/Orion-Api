using Dapper;
using Invest.Data.Exceptions;
using System;
using System.Data;

namespace Invest.Data.Repository.Generic
{
    public class BaseDapperRepository : IDisposable
    {
        protected readonly IDbConnection _connection;
        protected readonly DynamicParameters _dynamicParameters;

        public BaseDapperRepository(IDbConnection connection)
        {
            _connection = connection;
            _connection.Open();
            _dynamicParameters = new DynamicParameters();
        }

        public void AddParam(string name, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            DbType dataType;

            if (value is string)
            {
                dataType = DbType.String;
            }
            else if (value is decimal)
            {
                dataType = DbType.Decimal;
            }
            else if (value is DateTime)
            {
                dataType = DbType.DateTime;
            }
            else if (value is int)
            {
                dataType = DbType.Int32;
            }
            else if (value is long)
            {
                dataType = DbType.Int64;
            }
            else
            {
                throw new DatabaseExecption("No parameter data type configured for " + value.GetType());
            }

            _dynamicParameters.Add($"@{name}", value, dataType, direction);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
