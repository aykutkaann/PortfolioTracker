using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using PortfolioTracker.Application.DTOs.Auth;

namespace PortfolioTracker.Application.Validators
{
    public class LoginRequestValidator:AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
