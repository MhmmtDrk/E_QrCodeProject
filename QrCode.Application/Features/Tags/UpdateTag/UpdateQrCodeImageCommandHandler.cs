using MediatR;
using QrCode.Domain.AggregateModels.Tags.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Tags.UpdateTag
{
    public class UpdateQrCodeImageCommandHandler : IRequestHandler<UpdateQrCodeImageCommand>
    {
        private readonly ITagService _tagService;

        public UpdateQrCodeImageCommandHandler(ITagService tagService)
        {
            _tagService = tagService;
        }
        public async Task Handle(UpdateQrCodeImageCommand request, CancellationToken cancellationToken)
        {
            await _tagService.UpdateQrCodeImage(request.Tag, request.UserId);
        }
    }
}
