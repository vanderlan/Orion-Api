using FluentValidation;
using Microsoft.Extensions.Localization;
using Orion.Crosscutting.Resources;
using Orion.Crosscutting.Resources.Messages;

namespace Orion.Application.Core.Commands.UserChangePassword;

public class UserChangePasswordRequestValidator : AbstractValidator<UserChangePasswordRequest>
{
    public UserChangePasswordRequestValidator(IStringLocalizer<OrionResources> stringLocalizer)
    {

        RuleFor(c => c.CurrentPassword)
            .NotEmpty().WithMessage(stringLocalizer[MessagesKeys.UserMessages.EmptyPassword]);

        RuleFor(c => c.NewPassword)
            .NotEmpty().WithMessage(stringLocalizer[MessagesKeys.UserMessages.EmptyNewPassword]);

        RuleFor(c => c.NewPasswordConfirm)
            .NotEmpty().WithMessage(stringLocalizer[MessagesKeys.UserMessages.EmptyNewPasswordConfirmation]);

        RuleFor(x => x.NewPassword).Equal(x => x.NewPasswordConfirm)
            .WithMessage(stringLocalizer[MessagesKeys.UserMessages.PasswordAndConfirmationDifferent]);
    }
}
