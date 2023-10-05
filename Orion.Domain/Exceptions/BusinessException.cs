using System;
using System.Runtime.Serialization;

namespace Orion.Domain.Exceptions
{
    [Serializable]
    public class BusinessException : Exception
    {
        public string Title { get; set; }

        public BusinessException() : base()
        {
        }

        public BusinessException(string message) : base(message)
        {
        }

        public BusinessException(string message, string title) : base(message)
        {
            Title = title;
        }

        public BusinessException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BusinessException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
