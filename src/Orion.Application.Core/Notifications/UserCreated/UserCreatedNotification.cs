using MediatR;
using Orion.Domain.Core.Entities;

namespace Orion.Application.Core.Notifications.UserCreated
{
    public class UserCreatedNotification(User user) : INotification
    {
        public User Entity { get; set; } = user;
    }
}
