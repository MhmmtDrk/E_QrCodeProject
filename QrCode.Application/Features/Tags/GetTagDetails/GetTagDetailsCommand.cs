using MediatR;
using QrCode.Domain.AggregateModels.TagModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Tags.GetTagDetails
{  
    public sealed record GetTagDetailsCommand(string tag) : IRequest<Tag>;
}
