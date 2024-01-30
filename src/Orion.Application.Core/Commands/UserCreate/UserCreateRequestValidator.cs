using FluentValidation;
using Microsoft.Extensions.Localization;
using Orion.Crosscutting.Resources;
using Orion.Crosscutting.Resources.Messages;

namespace Orion.Application.Core.Commands.UserCreate;

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
