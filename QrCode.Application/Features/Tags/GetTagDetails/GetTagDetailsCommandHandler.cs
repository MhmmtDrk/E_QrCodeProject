using MediatR;
using QrCode.Application.Features.Tags.IsTagVerification;
using QrCode.Domain.AggregateModels.TagModels;
using QrCode.Domain.AggregateModels.Tags.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Tags.GetTagDetails
{
    public class GetTagDetailsCommandHandler: IRequestHandler<GetTagDetailsCommand, Tag>
    {
        private readonly ITagService _tagService;
        public GetTagDetailsCommandHandler(ITagService tagService)
        {
            _tagService = tagService;
        }
        public async Task<Tag> Handle(GetTagDetailsCommand request, CancellationToken cancellationToken)
        {
            var isVerification = await _tagService.GetTagDetails(request.tag);
            return isVerification;

        }
    }
}
