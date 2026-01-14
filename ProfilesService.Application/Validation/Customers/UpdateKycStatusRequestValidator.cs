using FluentValidation;
using ProfilesService.Application.Dtos.Customers;

namespace ProfilesService.Application.Validation.Customers
{
    public sealed class UpdateKycStatusRequestValidator : AbstractValidator<UpdateKycStatusRequest>
    {
        public UpdateKycStatusRequestValidator()
        {
            RuleFor(x => x.KycStatus)
                .IsInEnum()
                .WithMessage("Invalid KycStatus.");
        }
    }
}
