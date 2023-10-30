using MediatR;
using Orion.Domain.Core.Entities;

namespace Orion.Application.Core.Notifications.UserCreated
{
    public class UserCreatedNotification : INotification
    {
        public UserCreatedNotification(User user)
        {
            Entity = user;
        }

        public User Entity { get; set; }
    }
}
