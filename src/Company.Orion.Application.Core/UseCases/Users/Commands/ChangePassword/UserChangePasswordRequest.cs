using MediatR;

namespace Company.Orion.Application.Core.UseCases.Users.Commands.ChangePassword
{
    public class UserChangePasswordRequest : IRequest<Unit>
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }
    }
}
