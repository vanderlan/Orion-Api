using Company.Orion.Domain.Core.Authentication;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Company.Orion.Application.Core.UseCases.Users.Notifications.UserCreated
{
    public class UserCreatedNotificationHandler(
        ILogger<UserCreatedNotificationHandler> logger,
        ICurrentUser currentUser)
        : INotificationHandler<UserCreatedNotification>
    {
        private static readonly JsonSerializerOptions NotificationSerializerOptions = new() { WriteIndented = true };

        public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("A notification {NotificationType} has been received. Notification details: {Notification}. Action performed by: {CurrentUserName}",
                nameof(UserCreatedNotification),
                JsonSerializer.Serialize(notification, NotificationSerializerOptions),
                currentUser.ToString());

            await Task.CompletedTask;
        }
    }
}
