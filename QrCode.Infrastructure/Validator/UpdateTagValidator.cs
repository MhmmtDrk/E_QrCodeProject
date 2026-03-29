using FluentValidation;
using QrCode.Application.Features.Tags.UpdateTagDetails;
using QrCode.Application.Features.Users.CreateUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Infrastructure.Validator
{
    public class UpdateTagValidator : AbstractValidator<UpdateTagDetailsCommand>
    {
        public UpdateTagValidator()
        {
            RuleFor(x => x.TagName).NotEmpty().WithMessage("Name required.");
            RuleFor(x => x.TagName).NotEmpty().WithMessage("Name required.");
        }
    }
}
