using FluentValidation;
using Microsoft.Extensions.Localization;
using Orion.Croscutting.Resources;
using static Orion.Croscutting.Resources.Messages.MessagesKeys;

namespace Orion.Application.Core.Queries.UserGetById;

public class UserGetByIdRequestValidator : AbstractValidator<UserGetByIdRequest>
{
    public UserGetByIdRequestValidator(IStringLocalizer<OrionResources> stringLocalizer)
    {
        RuleFor(c => c)
            .NotNull()
            .WithMessage(stringLocalizer[UserMessages.NullEntity]);

        RuleFor(c => c.PublicId)
            .NotEmpty().WithMessage(stringLocalizer[UserMessages.EmptyId])
            .NotNull().WithMessage(stringLocalizer[UserMessages.EmptyId]);
    }
}
