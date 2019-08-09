using System;

namespace VBaseProject.Service.Exceptions
{
    [Serializable]
    public class ConflictException : BusinessException
    {
        protected ConflictException()
        {

        }

        public ConflictException(string message) : base(message)
        {

        }

        protected ConflictException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
