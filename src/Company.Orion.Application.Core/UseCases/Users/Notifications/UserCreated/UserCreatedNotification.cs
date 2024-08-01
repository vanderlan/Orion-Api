using Company.Orion.Domain.Core.Entities;
using MediatR;

namespace Company.Orion.Application.Core.UseCases.Users.Notifications.UserCreated
{
    public class UserCreatedNotification(User user) : INotification
    {
        public User Entity { get; set; } = user;
    }
}
