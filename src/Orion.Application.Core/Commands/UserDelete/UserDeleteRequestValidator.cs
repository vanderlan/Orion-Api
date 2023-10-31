using FluentValidation;
using Microsoft.Extensions.Localization;
using Orion.Croscutting.Resources;
using static Orion.Croscutting.Resources.Messages.MessagesKeys;

namespace Orion.Application.Core.Commands.UserDelete;

public class UserDeleteRequestValidator : AbstractValidator<UserDeleteRequest>
{
    public UserDeleteRequestValidator(IStringLocalizer<OrionResources> stringLocalizer)
    {
        RuleFor(c => c)
            .NotNull()
            .WithMessage(stringLocalizer[UserMessages.NullEntity]);

        RuleFor(c => c.PublicId)
            .NotEmpty().WithMessage(stringLocalizer[UserMessages.EmptyId])
            .NotNull().WithMessage(stringLocalizer[UserMessages.EmptyId]);
    }
}
