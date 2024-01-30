using System;

namespace Orion.Domain.Core.Exceptions;

[Serializable]
public class UnauthorizedUserException : BusinessException
{
    public UnauthorizedUserException(string message, string title) : base(message, title)
    {

    }

    public UnauthorizedUserException(string message) : base(message)
    {
    }

    public UnauthorizedUserException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
