using AccountsService.Application.Dtos.Accounts;
using FluentValidation;

namespace AccountsService.Application.Validation.Accounts
{
    public sealed class ChangeAccountStatusRequestValidator : AbstractValidator<ChangeAccountStatusRequest>
    {
        public ChangeAccountStatusRequestValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid AccountStatus.");
        }
    }
}
