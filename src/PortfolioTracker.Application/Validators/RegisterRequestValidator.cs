using FluentValidation;
using PortfolioTracker.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioTracker.Application.Validators
{
    public class RegisterRequestValidator :AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty().MinimumLength(6);

            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(50).MinimumLength(2);
        }
    }
}
