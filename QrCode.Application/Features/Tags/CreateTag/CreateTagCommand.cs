using MediatR;
using QrCode.Domain.AggregateModels.TagModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Tags.CreateTag
{
    public sealed record CreateTagCommand(
        string UserId,
        string No,
        string VerificationCode,
        string Description,
        bool IsVerification,
        bool IsActive,
        string QrCodeUrl,
        byte[] QrCodeImageUrl,
        string TagImages,
        string TagName,
        string TagBreed,
        string TagAge,
        string TagTelephoneNumber,
        string OrderNo,
        string OrderName,
        string OrderTelephone,
        string OrderSKU,
        DateTime CreatedDate,
        string CreatedUserId,
        string CustomerName,
        string Address, 
        bool IsLoss,
        string DogLeash,
        int TagType
        ) : IRequest<Tag>;
    

    
}
