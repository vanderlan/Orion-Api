using System;

namespace Orion.Domain.Core.Exceptions;

[Serializable]
public class NotFoundException : BusinessException
{
    public string Id { get; private set; }

    public NotFoundException(string id) : base($"Resource Not Found with id: {id}")
    {
        Id = id;
    }

    public NotFoundException() : base()
    {

    }

    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
