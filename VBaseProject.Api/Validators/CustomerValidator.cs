using FluentValidation;
using System;
using VBaseProject.Api.AutoMapper.Input;

namespace VBaseProject.Api.Validators
{
    public class CustomerValidator : AbstractValidator<CustomerInput>
    {
        public CustomerValidator()
        {
            RuleFor(c => c)
                .NotNull()
                .OnAnyFailure(x =>
                {
                    throw new ArgumentNullException("The Customer can't be found.");
                });

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Is necessary to inform the Customer Name.")
                .NotNull().WithMessage("Is necessary to inform the Customer Name.");

            RuleFor(c => c.Address)
               .NotEmpty().WithMessage("Is necessary to inform the Customer Address.")
               .NotNull().WithMessage("Is necessary to inform the Customer Address.");

            RuleFor(c => c.PhoneNumber)
                .NotEmpty().WithMessage("Is necessary to inform the Customer Phone Number.")
                .NotNull().WithMessage("Is necessary to inform the Customer Phone Number.");
        }
    }
}
