using MediatR;

namespace Orion.Application.Core.Commands.UserChangePassword
{
    public class UserChangePasswordRequest : IRequest<Unit>
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }
    }
}
