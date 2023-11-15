using FluentValidation;
using Microsoft.Extensions.Localization;
using Orion.Croscutting.Resources;
using static Orion.Croscutting.Resources.Messages.MessagesKeys;

namespace Orion.Application.Core.Commands.UserChangePassword;

public class UserChangePasswordRequestValidator : AbstractValidator<UserChangePasswordRequest>
{
    public UserChangePasswordRequestValidator(IStringLocalizer<OrionResources> stringLocalizer)
    {

        RuleFor(c => c.CurrentPassword)
            .NotEmpty().WithMessage(stringLocalizer[UserMessages.EmptyPasword]);

        RuleFor(c => c.NewPassword)
            .NotEmpty().WithMessage(stringLocalizer[UserMessages.EmptyNewPasword]);

        RuleFor(c => c.NewPasswordConfirm)
            .NotEmpty().WithMessage(stringLocalizer[UserMessages.EmptyNewPaswordConfirmation]);

        RuleFor(x => x.NewPassword).Equal(x => x.NewPasswordConfirm)
            .WithMessage(stringLocalizer[UserMessages.PaswordAndConfirmationDifferent]);
    }
}
