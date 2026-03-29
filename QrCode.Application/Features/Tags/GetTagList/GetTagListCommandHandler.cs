using MediatR;
using QrCode.Application.Features.Tags.GetUserTag;
using QrCode.Domain.AggregateModels.TagModels;
using QrCode.Domain.AggregateModels.Tags;
using QrCode.Domain.AggregateModels.Tags.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Tags.GetTagList
{
    public class GetTagListCommandHandler : IRequestHandler<GetTagListCommand, List<View_TagUser>>
    {
        private readonly ITagService _tagService;
        public GetTagListCommandHandler(ITagService tagService)
        {
            _tagService = tagService;
        }
        public async Task<List<View_TagUser>> Handle(GetTagListCommand request, CancellationToken cancellationToken)
        {
            var tagList = await _tagService.GetTagList();
            return tagList;

        }
    }
}
