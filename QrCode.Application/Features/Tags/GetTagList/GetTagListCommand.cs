using MediatR;
using QrCode.Domain.AggregateModels.TagModels;
using QrCode.Domain.AggregateModels.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Tags.GetTagList
{
    public sealed record GetTagListCommand() : IRequest<List<View_TagUser>>;
}
