using MediatR;
using QrCode.Domain.AggregateModels.TagModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Tags.VerificationCode
{
    public sealed record GetVerificationCode(string tag) : IRequest<string>;

}
