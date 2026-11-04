using Company.Orion.Domain.Core.Authentication;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Company.Orion.Application.Core.UseCases.Users.Notifications.UserPasswordChanged
{
    public class UserPasswordChangedNotificationHandler(
        ILogger<UserPasswordChangedNotificationHandler> logger,
        ICurrentUser currentUser)
        : INotificationHandler<UserPasswordChangedNotification>
    {
        private static readonly JsonSerializerOptions NotificationSerializerOptions = new() { WriteIndented = true };

        public async Task Handle(UserPasswordChangedNotification notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("A notification {NotificationType} has been received. Notification details: {Notification}. Action performed by: {CurrentUserName}",
                nameof(UserPasswordChangedNotification),
                JsonSerializer.Serialize(notification, NotificationSerializerOptions),
                currentUser.ToString());

            await Task.CompletedTask;
        }
    }
}
