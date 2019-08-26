using FluentValidation;
using Microsoft.Extensions.Localization;
using System;
using VBaseProject.Api.AutoMapper.Input;
using VBaseProject.Resources;

namespace VBaseProject.Api.Validators
{
    public class CustomerValidator : AbstractValidator<CustomerInput>
    {
        public CustomerValidator(IStringLocalizer<VBaseProjectResources> stringLocalizer)
        {

            RuleFor(c => c)
                .NotNull()
                .OnAnyFailure(x =>
                {
                    throw new ArgumentNullException(stringLocalizer["Customer.NullEntity"]);
                });

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage(stringLocalizer["Customer.InvalidName"])
                 .NotNull().WithMessage(stringLocalizer["Customer.InvalidName"]);

            RuleFor(c => c.Address)
               .NotEmpty().WithMessage(stringLocalizer["Customer.InvalidAddress"])
                .NotNull().WithMessage(stringLocalizer["Customer.InvalidAddress"]);

            RuleFor(c => c.PhoneNumber)
                .NotEmpty().WithMessage(stringLocalizer["Customer.InvalidPhoneNumber"])
                 .NotNull().WithMessage(stringLocalizer["Customer.InvalidPhoneNumber"]);
        }
    }
}
