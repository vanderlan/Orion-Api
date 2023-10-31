using FluentValidation;
using Microsoft.Extensions.Localization;
using Orion.Croscutting.Resources;
using static Orion.Croscutting.Resources.Messages.MessagesKeys;

namespace Orion.Application.Core.Commands.UserUpdate;

public class UserUpdateRequestValidator : AbstractValidator<UserUpdateRequest>
{
    public UserUpdateRequestValidator(IStringLocalizer<OrionResources> stringLocalizer)
    {
        RuleFor(c => c)
            .NotNull()
            .WithMessage(stringLocalizer[UserMessages.NullEntity]);

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage(stringLocalizer[UserMessages.EmptyName])
             .NotNull().WithMessage(stringLocalizer[UserMessages.EmptyName]);
        
        RuleFor(c => c.Email)
            .NotEmpty().WithMessage(stringLocalizer[UserMessages.EmptyEmail])
            .NotNull().WithMessage(stringLocalizer[UserMessages.EmptyEmail]);
        
        RuleFor(c => c.PublicId)
            .NotEmpty().WithMessage(stringLocalizer[UserMessages.EmptyId])
            .NotNull().WithMessage(stringLocalizer[UserMessages.EmptyId]);
    }
}
