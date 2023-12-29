﻿using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orion.Domain.Core.Authentication;

namespace Orion.Application.Core.Notifications.UserPasswordChanged
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
