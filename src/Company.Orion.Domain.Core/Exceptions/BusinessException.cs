namespace Company.Orion.Domain.Core.Exceptions;

public class BusinessException : Exception
{
    /// <summary>Optional title shown to the client (ProblemDetails.Title). Defaults to the HTTP status title when null.</summary>
    public string? Title { get; set; }

    public BusinessException(string message, string? title) : base(message)
    {
        Title = title;
    }

    public BusinessException(string message) : base(message)
    {
    }

    public BusinessException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
