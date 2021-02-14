namespace VBaseProject.Service.Exceptions
{
    public class ExceptionResponse
    {
        public ExceptionResponse(string message, NotificationType notificationType)
        {
            Message = message;
            NotificationType = notificationType.ToString().ToUpperInvariant();
        }

        public string Message { get; set; }
        public string Title { get; set; }
        public string NotificationType { get; set; }
    }

    public enum NotificationType
    {
        Info = 1,
        Warnning = 2,
        Error = 3
    }
}
