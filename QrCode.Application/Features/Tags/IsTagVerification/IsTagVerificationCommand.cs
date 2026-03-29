using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Tags.IsTagVerification
{
    public sealed record IsTagVerificationCommand(string tag):IRequest<bool>;
  
}
