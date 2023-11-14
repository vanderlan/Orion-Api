using MediatR;
using Orion.Application.Core.Notifications.UserPasswordChanged;
using Orion.Domain.Core.Authentication;
using Orion.Domain.Core.Services.Interfaces;

namespace Orion.Application.Core.Commands.UserChangePassword
{
    public class UserUpdatePasswordRequestHandler : IRequestHandler<UserChangePasswordRequest, Unit>
    {
        private readonly IUserService _userService;
        private readonly ICurrentUser _currentUser;
        private readonly IMediator _mediator;

        public UserUpdatePasswordRequestHandler(IUserService userService, ICurrentUser currentUser, IMediator mediator)
        {
            _userService = userService;
            _currentUser = currentUser;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(UserChangePasswordRequest request, CancellationToken cancellationToken)
        {
            await _userService.ChangePasswordAsync(_currentUser.Id, request.CurrentPassword, request.NewPassword);

            await _mediator.Publish(new UserPasswordChangedNotification(), cancellationToken);

            return Unit.Value;
        }
    }
}
