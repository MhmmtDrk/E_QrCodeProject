using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using QrCode.Application.Features.Users;
using QrCode.Application.Features.Users.CreateUser;

namespace QrCode.Domain.Validator
{
    public class UserCreateValidator:AbstractValidator<CreateUserCommand>
    {
        public UserCreateValidator() 
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name required.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("A valid email address is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password required.");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("ConfirmPassword required.");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match.");
        }
    }
}
