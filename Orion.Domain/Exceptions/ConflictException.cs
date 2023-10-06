using System;
using System.Runtime.Serialization;

namespace Orion.Domain.Exceptions
{
    [Serializable]
    public class ConflictException : BusinessException
    {
        public ConflictException(string message, string title) : base(message, title)
        {

        }

        public ConflictException() : base()
        {

        }

        public ConflictException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ConflictException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
