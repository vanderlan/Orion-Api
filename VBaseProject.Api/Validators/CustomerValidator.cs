using FluentValidation;
using Microsoft.Extensions.Localization;
using VBaseProject.Api.AutoMapper.Input;
using VBaseProject.Resources;
using static VBaseProject.Resources.Messages.MessagesKeys;

namespace VBaseProject.Api.Validators
{
    public class CustomerValidator : AbstractValidator<CustomerInput>
    {
        public CustomerValidator(IStringLocalizer<VBaseProjectResources> stringLocalizer)
        {
            RuleFor(c => c)
                .NotNull()
                .WithMessage(stringLocalizer[CustomerMessages.NullEntity]);

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage(stringLocalizer[CustomerMessages.InvalidName])
                 .NotNull().WithMessage(stringLocalizer[CustomerMessages.InvalidName]);
        }
    }
}
