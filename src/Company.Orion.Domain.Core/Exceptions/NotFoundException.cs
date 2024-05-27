using System;

namespace Company.Orion.Domain.Core.Exceptions;

[Serializable]
public class NotFoundException : BusinessException
{
    public NotFoundException(string id) : base($"Resource Not Found with id: {id}")
    {
        
    }

    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
