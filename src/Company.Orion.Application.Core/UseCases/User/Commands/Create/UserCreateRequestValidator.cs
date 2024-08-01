using FluentValidation;
using Microsoft.Extensions.Localization;
using Company.Orion.Crosscutting.Resources;
using Company.Orion.Crosscutting.Resources.Messages;

namespace Company.Orion.Application.Core.UseCases.User.Commands.Create;

public class UserCreateRequestValidator : AbstractValidator<UserCreateRequest>
{
    public UserCreateRequestValidator(IStringLocalizer<OrionResources> stringLocalizer)
    {
        RuleFor(c => c)
            .NotNull()
            .WithMessage(stringLocalizer[MessagesKeys.UserMessages.NullEntity]);

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage(stringLocalizer[MessagesKeys.UserMessages.EmptyName]);

        RuleFor(c => c.Email)
            .NotEmpty().WithMessage(stringLocalizer[MessagesKeys.UserMessages.EmptyEmail]);

        RuleFor(c => c.Password)
            .NotEmpty().WithMessage(stringLocalizer[MessagesKeys.UserMessages.EmptyPassword]);
    }
}
