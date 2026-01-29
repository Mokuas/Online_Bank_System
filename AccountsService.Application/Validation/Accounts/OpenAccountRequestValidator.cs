using AccountsService.Application.Common;
using AccountsService.Application.Dtos.Accounts;
using FluentValidation;

namespace AccountsService.Application.Validation.Accounts
{
    public sealed class OpenAccountRequestValidator : AbstractValidator<OpenAccountRequest>
    {
        public OpenAccountRequestValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("CustomerId is required.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid account type.");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required.")
                .Length(3).WithMessage("Currency must be a 3-letter code.")
                .Matches("^[A-Z]{3}$").WithMessage("Currency must be uppercase (e.g., USD).");
        }
    }
}
