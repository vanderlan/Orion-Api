using System;

namespace VBaseProject.Service.Exceptions
{
    [Serializable]
    public class DatabaseException : BusinessException
    {
        public DatabaseException()
        {
        }

        public DatabaseException(string message) : base(message)
        {
        }
    }
}
