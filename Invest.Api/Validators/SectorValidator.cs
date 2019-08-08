using FluentValidation;
using Invest.Api.AutoMapper;
using System;

namespace Invest.Api.Validators
{
    public class SectorValidator : AbstractValidator<SectorInput>
    {
        public SectorValidator()
        {
            RuleFor(c => c)
                .NotNull()
                .OnAnyFailure(x =>
                {
                    throw new ArgumentNullException("The Sector can't be found.");
                });

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Is necessary to inform the Sector Name.")
                .NotNull().WithMessage("Is necessary to inform the Sector Name.");

            RuleFor(c => c.StockExchangeId)
                .NotEmpty().WithMessage("Is necessary to inform the StockExchangeId.")
                .NotNull().WithMessage("Is necessary to inform the StockExchangeId.");
        }
    }
}
