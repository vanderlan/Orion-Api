using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orion.Domain.Core.Authentication;

namespace Orion.Application.Core.Notifications.UserCreated
{
    public class UserCreatedNotificationHandler(
        ILogger<UserCreatedNotificationHandler> logger,
        ICurrentUser currentUser)
        : INotificationHandler<UserCreatedNotification>
    {
        public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("A notification {notificationType} has been received. Notification details: {notification}. Action performed by: {currentUserName}", 
                nameof(UserCreatedNotification),
                JsonConvert.SerializeObject(notification, Formatting.Indented),
                currentUser.ToString());

            await Task.CompletedTask;
        }
    }
}
