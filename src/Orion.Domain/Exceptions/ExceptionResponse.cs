namespace Orion.Domain.Exceptions;

public class ExceptionResponse
{
    public ExceptionResponse(string message, NotificationType notificationType)
    {
        Errors = new string[] { message };
        NotificationType = notificationType.ToString().ToUpperInvariant();
    }

    public ExceptionResponse(string [] errors, NotificationType notificationType)
    {
        Errors = errors ;
        NotificationType = notificationType.ToString().ToUpperInvariant();
    }
    public string Title { get; set; }
    public string NotificationType { get; set; }
    public string [] Errors { get; set; }
}

public enum NotificationType
{
    Info = 1,
    Warnning = 2,
    Error = 3
}
