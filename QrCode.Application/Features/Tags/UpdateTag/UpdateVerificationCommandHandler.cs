using MediatR;
using QrCode.Domain.AggregateModels.Tags.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Tags.UpdateTag
{
    public class UpdateVerificationCommandHandler: IRequestHandler<UpdateVerificationCommand>
    {
        private readonly ITagService _tagService;

        public UpdateVerificationCommandHandler(ITagService tagService)
        {
            _tagService = tagService;
        }
        public async Task Handle(UpdateVerificationCommand request, CancellationToken cancellationToken)
        {
            await _tagService.UpdateVerification(request.Tag,request.UserId);
        }
    }
}
