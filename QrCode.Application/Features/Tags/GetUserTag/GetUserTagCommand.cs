using MediatR;
using QrCode.Domain.AggregateModels.TagModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Tags.GetUserTag
{
    public sealed record GetUserTagCommand(string UserId): IRequest<List<Tag>>;

}
