using Company.Orion.Application.Core.UseCases.Users.Notifications.UserPasswordChanged;
using Company.Orion.Domain.Core.Authentication;
using Company.Orion.Domain.Core.Services.Interfaces;
using MediatR;

namespace Company.Orion.Application.Core.UseCases.Users.Commands.ChangePassword
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
