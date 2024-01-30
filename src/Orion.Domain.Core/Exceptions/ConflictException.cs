using System;

namespace Orion.Domain.Core.Exceptions;

[Serializable]
public class ConflictException : BusinessException
{
    public ConflictException(string message, string title) : base(message, title)
    {

    }

    public ConflictException(string message) : base(message)
    {

    }

    public ConflictException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
