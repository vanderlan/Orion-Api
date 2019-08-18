using System;

namespace VBaseProject.Service.Exceptions
{
    [Serializable]
    public class BusinessException : Exception
    {
        public string Title { get; set; }
        public BusinessException()
        {

        }

        public BusinessException(string message) : base(message)
        {

        }

        public BusinessException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
