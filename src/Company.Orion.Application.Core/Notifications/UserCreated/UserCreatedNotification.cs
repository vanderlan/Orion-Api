using MediatR;
using Company.Orion.Domain.Core.Entities;

namespace Company.Orion.Application.Core.Notifications.UserCreated
{
    public class UserCreatedNotification(User user) : INotification
    {
        public User Entity { get; set; } = user;
    }
}
