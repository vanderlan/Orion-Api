using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Orion.Application.Core.Notifications.UserCreated
{
    public class UserCreatedNotificationHandler : INotificationHandler<UserCreatedNotification>
    {
        private readonly ILogger<UserCreatedNotificationHandler> _logger;

        public UserCreatedNotificationHandler(ILogger<UserCreatedNotificationHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("A notification {notificationType} has been received. Notification details: {notification}", 
                nameof(UserCreatedNotification),
                JsonConvert.SerializeObject(notification, Formatting.Indented));

            await Task.CompletedTask;
        }
    }
}
