using Company.Orion.Crosscutting.Resources;
using Company.Orion.Crosscutting.Resources.Messages;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Company.Orion.Application.Core.UseCases.Users.Commands.Update;

public class UserUpdateRequestValidator : AbstractValidator<UserUpdateRequest>
{
    public UserUpdateRequestValidator(IStringLocalizer<OrionResources> stringLocalizer)
    {
        RuleFor(c => c)
            .NotNull()
            .WithMessage(stringLocalizer[MessagesKeys.UserMessages.NullEntity]);

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage(stringLocalizer[MessagesKeys.UserMessages.EmptyName]);

        RuleFor(c => c.Email)
            .NotEmpty().WithMessage(stringLocalizer[MessagesKeys.UserMessages.EmptyEmail]);

        RuleFor(c => c.PublicId)
            .NotEmpty().WithMessage(stringLocalizer[MessagesKeys.UserMessages.EmptyId]);
    }
}
