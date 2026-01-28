using FluentValidation;
using ProfilesService.Application.Dtos.Customers;
using ProfilesService.Application.Validation.Common;
using ProfilesService.Application.Common;

namespace ProfilesService.Application.Validation.Customers
{
    public sealed class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
    {
        public CreateCustomerRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("FirstName is required.")
                .MaximumLength(FieldConstraints.NameMaxLength);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName is required.")
                .MaximumLength(FieldConstraints.NameMaxLength);

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(FieldConstraints.AddressMaxLength);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email format is invalid.")
                .MaximumLength(FieldConstraints.EmailMaxLength);

            RuleFor(x => x.PhoneNumber)
                .ValidPhoneNumber();

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("DateOfBirth is required.");
        }
    }
}
