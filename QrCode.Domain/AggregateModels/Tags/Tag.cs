using MediatR.Pipeline;
using QrCode.Domain.AggregateModels.Users;
using QrCode.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Domain.AggregateModels.TagModels
{
    public class Tag:BaseEntity
    {
        private Tag(Guid id) : base(id)
        {
            
        }
        private Tag(Guid id,string userId, string no, string verificationCode, string description, bool isVerification, bool isActive, string qrCodeUrl, byte[] qrCodeImageUrl, string tagImages,string tagName, string tagBreed, string tagAge, string tagTelephoneNumber,string orderNo,string orderName,string orderTelephone,string orderSKU,DateTime createdDate, string createdUserId,string customerName,string address,bool isLoss,string dogleash,int tagType) : base(id)
        {
            UserId = userId;
            No = no;
            VerificationCode = verificationCode;
            Description = description;
            IsVerification = isVerification;
            IsActive = isActive;
            QrCodeUrl = qrCodeUrl;
            QrCodeImageUrl = qrCodeImageUrl;            
            TagImages = tagImages;
            TagBreed = tagBreed;
            TagAge = tagAge;
            TagTelephoneNumber = tagTelephoneNumber;
            TagName = tagName;
            OrderNo= orderNo;
            OrderName= orderName;
            OrderTelephone = orderTelephone;
            OrderSKU = orderSKU;
            CreatedDate= createdDate;
            CreatedUserId= createdUserId;
            CustomerName= customerName;
            Address= address;
            IsLoss= isLoss;
            DogLeash = dogleash;
            TagType = tagType;


        }
        public string UserId { get; private set; }
        public string? No { get; private set; }      
        public string? VerificationCode { get;private set; }
        public string? Description { get; private set; }
        public bool IsVerification { get; private set; }
        public bool IsActive { get; private set; }
        public string? QrCodeUrl { get; private set; }
        public byte[]? QrCodeImageUrl { get; private set; }
        public string? TagImages { get; private set; }
        public string? TagName { get; private set; }
        public string? TagBreed { get; private set; }
        public string? TagAge { get; private set; }
        public string? TagTelephoneNumber { get; private set; }
        public string? OrderNo { get; private set; }
        public string? OrderName { get; private set; }
        public string? OrderTelephone { get; private set; }
        public string? OrderSKU { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedUserId { get; set; }
        public string? CustomerName { get; set; }
        public string? Address { get; set; }
        public bool IsLoss { get; set; }
        public string? DogLeash { get; set; }
        public int TagType { get; set; }
        public static Tag CreateTag(string userId, string no, string verificationCode,string description,bool isVerification,bool isActive, string qrCodeUrl,byte[] qrCodeImageUrl, string tagImages,string tagName, string tagBreed, string tagAge, string tagTelephoneNumber,string orderNo,string orderName,string orderTelephone,string orderSKU,DateTime createdDate,string createdUserId,string customerName,string address, bool isLoss,string dogLeash,int tagType)
        {
            Tag tag = new(
                id: Guid.NewGuid(),
                userId: userId,
                no: new(no),
                verificationCode: new(verificationCode),
                description: new(description),
                isVerification: isVerification,
                isActive: isActive,
                qrCodeUrl:new(qrCodeUrl),
                qrCodeImageUrl:qrCodeImageUrl,
                tagImages:tagImages,
                tagName:tagName,
                tagBreed:tagBreed,
                tagAge:tagAge,
                tagTelephoneNumber:tagTelephoneNumber,
                orderNo:orderNo,
                orderName:orderName,
                orderTelephone:orderTelephone,
                orderSKU:orderSKU,
                createdDate:createdDate,
                createdUserId:createdUserId,
                customerName:customerName,
                address:address,
                isLoss:isLoss,
                dogleash:dogLeash,
                tagType:tagType
                );
            return tag;
        }
        public void UpdateVerification(string userId)
        {
            IsVerification = true;
            UserId= userId;
        }
        public void UpdateQrCodeImage()
        {
            QrCodeImageUrl = null;
        }
        public void UpdateTagDetails(string? description, string tagImages, string? tagName, string? tagBreed, string? tagAge, string? tagTelephoneNumber,bool isLoss)
        {
            Description = description;           
            TagImages = tagImages;
            TagName = tagName;
            TagBreed = tagBreed;
            TagAge = tagAge;
            TagTelephoneNumber = tagTelephoneNumber;
            IsLoss = isLoss;
            
           
        }

    }
}
