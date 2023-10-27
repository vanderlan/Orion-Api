using System;
using System.Runtime.Serialization;

namespace Orion.Core.Domain.Exceptions;

[Serializable]
public class UnauthorizedUserException : BusinessException
{
    public UnauthorizedUserException(string message, string title) : base(message, title)
    {

    }

    public UnauthorizedUserException() : base()
    {
    }

    public UnauthorizedUserException(string message) : base(message)
    {
    }

    public UnauthorizedUserException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected UnauthorizedUserException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
