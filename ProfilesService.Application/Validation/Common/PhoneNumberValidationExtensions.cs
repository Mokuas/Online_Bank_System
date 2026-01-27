using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProfilesService.Application.Validation.Common
{
    public static class PhoneNumberValidationExtensions
    {
        private const string PhoneRegex = @"^\+?[0-9\s\-]{7,20}$";

        public static IRuleBuilderOptions<T, string> ValidPhoneNumber<T>(
            this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("PhoneNumber is required.")
                .Matches(PhoneRegex).WithMessage("PhoneNumber format is invalid.");
        }
    }
}
