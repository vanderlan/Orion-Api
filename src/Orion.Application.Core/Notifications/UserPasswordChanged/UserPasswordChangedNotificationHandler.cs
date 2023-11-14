using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orion.Domain.Core.Authentication;

namespace Orion.Application.Core.Notifications.UserPasswordChanged
{
    public class UserPasswordChangedNotificationHandler : INotificationHandler<UserPasswordChangedNotification>
    {
        private readonly ILogger<UserPasswordChangedNotificationHandler> _logger;
        private readonly ICurrentUser _currentUser;

        public UserPasswordChangedNotificationHandler(ILogger<UserPasswordChangedNotificationHandler> logger, ICurrentUser currentUser)
        {
            _logger = logger;
            _currentUser = currentUser;
        }

        public async Task Handle(UserPasswordChangedNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("A notification {notificationType} has been received. Notification details: {notification}. Action performed by: {currentUserName}", 
                nameof(UserPasswordChangedNotification),
                JsonConvert.SerializeObject(notification, Formatting.Indented),
                _currentUser.ToString());

            await Task.CompletedTask;
        }
    }
}
