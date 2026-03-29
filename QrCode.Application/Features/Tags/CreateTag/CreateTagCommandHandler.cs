using MediatR;
using QrCode.Application.Features.Tags.CreateTag;
using QrCode.Domain.AggregateModels.TagModels;
using QrCode.Domain.AggregateModels.Tags;
using QrCode.Domain.AggregateModels.Tags.Events;
using QrCode.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Tags.CreateTag
{
    internal sealed class CreateTagCommandHandler: IRequestHandler<CreateTagCommand,Tag>
    {
        private readonly ITagService _tagService;
        private readonly IUnitOfWork _UnitOfWork;
        public CreateTagCommandHandler(ITagService tagService, IUnitOfWork unitOfWork)
        {
            this._tagService = tagService;
            this._UnitOfWork = unitOfWork;
        }

        public async Task<Tag> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            //business kontrolleri yapabilirsin
            var tag=await _tagService.CreateAsync(request.UserId,request.No,request.VerificationCode,request.Description,request.IsVerification,request.IsActive,request.QrCodeUrl,request.QrCodeImageUrl,request.TagImages,request.TagName,request.TagBreed,request.TagAge,request.TagTelephoneNumber,request.OrderNo,request.OrderName,request.OrderTelephone,request.OrderSKU,request.CreatedUserId,request.CustomerName,request.Address,request.IsLoss,request.DogLeash,request.TagType,  cancellationToken);

            await _UnitOfWork.SaveChangesAsync(cancellationToken);
            return tag;
        }
    }
}
