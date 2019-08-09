using FluentValidation;
using VBaseProject.Api.AutoMapper;
using System;

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

            RuleFor(c => c.Code)
                .NotEmpty().WithMessage("Is necessary to inform the Customer Code.")
                .NotNull().WithMessage("Is necessary to inform the Customer Code.");

            RuleFor(c => c.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Customer Price has to be greater or equal to zero.")
                .NotNull().WithMessage("Is necessary to inform the Price.");

            RuleFor(c => c.StockExchangeId)
                .NotEmpty().WithMessage("Is necessary to inform the StockExchangeId.")
                .NotNull().WithMessage("Is necessary to inform the StockExchangeId.");
        }
    }
}
