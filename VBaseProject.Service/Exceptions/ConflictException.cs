using System;

namespace VBaseProject.Service.Exceptions
{
    [Serializable]
    public class ConflictException : BusinessException
    {
        protected ConflictException()
        {

        }

        public ConflictException(string message, string title) : base(message, title)
        {

        }

        protected ConflictException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
