using MediatR;
using QrCode.Domain.AggregateModels.TagModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Domain.AggregateModels.Tags.Events
{
    public interface ITagService
    {
        Task<Tag> CreateAsync(string userId, string no, string verificationCode, string description, bool isVerification, bool isActive, string qrCodeUrl, byte[] qrCodeImageUrl, string tagImages, string tagName, string tagBreed, string tagAge, string tagTelephoneNumber,string orderNo,string orderName,string OrderTelephone,string orderSKU,string createdUserId,string customerName,string address, bool isLoss,string dogLeash,int tagType, CancellationToken cancellationToken = default);
        Task UpdateVerification(string tag,string userId);
        Task CreateException(string exStackTrace, string exMessage,string sourceContext);
        Task UpdateQrCodeImage(string tag, string userId);
        Task<List<Tag>> GetUserTag(string userId);
        Task<byte[]> GenerateQRCode(string text);
        Task<string> GenerateTagNo(int lenght = 6);
        Task<string> GenerateVerificaionCode(int lenght = 6);
        Task<string> GetVerificationCodeByTag(string tag);
        Task UpdateTagDetails(string tagNo,string tagName,string tagBreed,string tagAge, string tagImages,string description,string tagTelephoneNumber,bool isLoss);
        Task<bool> IsTagVerification(string tag);
        Task<Tag> GetTagDetails(string tag);

        Task<List<View_TagUser>> GetTagList();
    }
}
