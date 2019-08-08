using FluentValidation;
using Invest.Api.AutoMapper;
using System;

namespace Invest.Api.Validators
{
    public class AssetValidator : AbstractValidator<AssetInput>
    {
        public AssetValidator()
        {
            RuleFor(c => c)
                .NotNull()
                .OnAnyFailure(x =>
                {
                    throw new ArgumentNullException("The Asset can't be found.");
                });

            RuleFor(c => c.Code)
                .NotEmpty().WithMessage("Is necessary to inform the Asset Code.")
                .NotNull().WithMessage("Is necessary to inform the Asset Code.");

            RuleFor(c => c.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Asset Price has to be greater or equal to zero.")
                .NotNull().WithMessage("Is necessary to inform the Price.");

            RuleFor(c => c.StockExchangeId)
                .NotEmpty().WithMessage("Is necessary to inform the StockExchangeId.")
                .NotNull().WithMessage("Is necessary to inform the StockExchangeId.");
        }
    }
}
