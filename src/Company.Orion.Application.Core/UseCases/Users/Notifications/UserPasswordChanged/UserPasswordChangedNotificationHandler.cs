using Company.Orion.Domain.Core.Authentication;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Company.Orion.Application.Core.UseCases.Users.Notifications.UserPasswordChanged
{
    public class UserPasswordChangedNotificationHandler(
        ILogger<UserPasswordChangedNotificationHandler> logger,
        ICurrentUser currentUser)
        : INotificationHandler<UserPasswordChangedNotification>
    {
        public async Task Handle(UserPasswordChangedNotification notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("A notification {notificationType} has been received. Notification details: {notification}. Action performed by: {currentUserName}",
                nameof(UserPasswordChangedNotification),
                JsonConvert.SerializeObject(notification, Formatting.Indented),
                currentUser.ToString());

            await Task.CompletedTask;
        }
    }
}
