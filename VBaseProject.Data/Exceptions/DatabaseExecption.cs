using System.Data.Common;

namespace VBaseProject.Data.Exceptions
{
    public class DatabaseExecption : DbException
    {
        public DatabaseExecption(string message) : base(message)
        {
        }
    }
}
