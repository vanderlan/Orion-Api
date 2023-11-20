using System;

namespace Orion.Domain.Core.Exceptions;

[Serializable]
public class BusinessException : Exception
{
    public string Title { get; set; }

    public BusinessException(string message, string title) : base(message)
    {
        Title = title;
    }
    
    public BusinessException() : base()
    {
    }

    public BusinessException(string message) : base(message)
    {
    }

    public BusinessException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
