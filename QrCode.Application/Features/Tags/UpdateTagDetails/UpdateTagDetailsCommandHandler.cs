using MediatR;
using QrCode.Application.Features.Tags.UpdateTag;
using QrCode.Domain.AggregateModels.Tags.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Tags.UpdateTagDetails
{
    public class UpdateTagDetailsCommandHandler: IRequestHandler<UpdateTagDetailsCommand>
    {
        private readonly ITagService _tagService;
        public UpdateTagDetailsCommandHandler(ITagService tagService)
        {
            _tagService = tagService;
        }
        public async Task Handle(UpdateTagDetailsCommand request, CancellationToken cancellationToken)
        {          
            await _tagService.UpdateTagDetails(request.Tag, request.TagName, request.TagBreed, request.TagAge, request.TagImages, request.Description, request.TagTelephoneNumber,request.IsLoss);
        }
    }
}
