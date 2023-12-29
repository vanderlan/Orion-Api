using MediatR;
using Orion.Application.Core.Notifications.UserPasswordChanged;
using Orion.Domain.Core.Authentication;
using Orion.Domain.Core.Services.Interfaces;

namespace Orion.Application.Core.Commands.UserChangePassword
{
    public class UserUpdatePasswordRequestHandler(
        IUserService userService,
        ICurrentUser currentUser,
        IMediator mediator)
        : IRequestHandler<UserChangePasswordRequest, Unit>
    {
        public async Task<Unit> Handle(UserChangePasswordRequest request, CancellationToken cancellationToken)
        {
            await userService.ChangePasswordAsync(currentUser.Id, request.CurrentPassword, request.NewPassword);

            await mediator.Publish(new UserPasswordChangedNotification(), cancellationToken);

            return Unit.Value;
        }
    }
}
