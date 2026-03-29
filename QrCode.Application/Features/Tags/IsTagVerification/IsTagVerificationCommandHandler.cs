using MediatR;
using QrCode.Application.Features.Tags.UpdateTagDetails;
using QrCode.Domain.AggregateModels.Tags.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Tags.IsTagVerification
{
    public class IsTagVerificationCommandHandler: IRequestHandler<IsTagVerificationCommand,bool>
    {
        private readonly ITagService _tagService;
        public IsTagVerificationCommandHandler(ITagService tagService)
        {
            _tagService = tagService;
        }
        public async Task<bool> Handle(IsTagVerificationCommand request, CancellationToken cancellationToken)
        {
            var isVerification=await _tagService.IsTagVerification(request.tag);
            return isVerification;

        }
    }
}
