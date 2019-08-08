using System.Data.Common;

namespace Invest.Data.Exceptions
{
    public class DatabaseExecption : DbException
    {
        public DatabaseExecption(string message) : base(message)
        {
        }
    }
}
