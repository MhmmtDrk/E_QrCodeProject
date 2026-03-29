using MediatR;
using QrCode.Application.Features.Tags.GetTagDetails;
using QrCode.Domain.AggregateModels.TagModels;
using QrCode.Domain.AggregateModels.Tags.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Tags.GetUserTag
{
    public class GetUserTagCommandHandler: IRequestHandler<GetUserTagCommand,List<Tag>>
    {
        private readonly ITagService _tagService;
        public GetUserTagCommandHandler(ITagService tagService)
        {
            _tagService = tagService;
        }
        public async Task<List<Tag>> Handle(GetUserTagCommand request, CancellationToken cancellationToken)
        {
            var userTag = await _tagService.GetUserTag(request.UserId);
            return userTag;

        }
    }
}
