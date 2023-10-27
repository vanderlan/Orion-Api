using FluentValidation;
using Microsoft.Extensions.Localization;
using Orion.Api.AutoMapper.Input;
using Orion.Croscutting.Resources;
using static Orion.Croscutting.Resources.Messages.MessagesKeys;

namespace Orion.Api.Validators;

public class CustomerValidator : AbstractValidator<CustomerInput>
{
    public CustomerValidator(IStringLocalizer<OrionResources> stringLocalizer)
    {
        RuleFor(c => c)
            .NotNull()
            .WithMessage(stringLocalizer[CustomerMessages.NullEntity]);

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage(stringLocalizer[CustomerMessages.InvalidName])
             .NotNull().WithMessage(stringLocalizer[CustomerMessages.InvalidName]);
    }
}
