using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orion.Domain.Core.Authentication;

namespace Orion.Application.Core.Notifications.UserCreated
{
    public class UserCreatedNotificationHandler : INotificationHandler<UserCreatedNotification>
    {
        private readonly ILogger<UserCreatedNotificationHandler> _logger;
        private readonly ICurrentUser _currentUser;

        public UserCreatedNotificationHandler(ILogger<UserCreatedNotificationHandler> logger, ICurrentUser currentUser)
        {
            _logger = logger;
            _currentUser = currentUser;
        }

        public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("A notification {notificationType} has been received. Notification details: {notification}. Action performed by: {currentUserName}", 
                nameof(UserCreatedNotification),
                JsonConvert.SerializeObject(notification, Formatting.Indented),
                _currentUser.ToString());

            await Task.CompletedTask;
        }
    }
}
