using QrCode.Domain.AggregateModels.TagModels;
using QrCode.Domain.AggregateModels.Tags;
using QrCode.Domain.AggregateModels.Users;
using QrCode.Infrastructure.Context;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;
using System.IO;
using Microsoft.EntityFrameworkCore;
using QrCode.Domain.AggregateModels.Tags.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Drawing.Imaging;

namespace QrCode.Infrastructure.Service
{
    internal class TagService : ITagService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TagService> _logger;
        public TagService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, ILogger<TagService> logger)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        public async Task<Tag> CreateAsync(string userId, string no, string verificationCode, string description, bool isVerification, bool isActive, string qrCodeUrl, byte[] qrCodeImageUrl, string tagImages, string tagName, string tagBreed, string tagAge, string tagTelephoneNumber,string orderNo,string orderName,string orderTelephone,string orderSKU,string createdUserId,string customerName,string address, bool isLoss,string dogLeash,int tagType, CancellationToken cancellationToken = default)
        {
            var tagNo = "PNG";
            if (tagType!=3)
            {
                tagNo = await GenerateTagNo();
                while (IsNumberExist(tagNo))
                {
                    tagNo = await GenerateTagNo();
                }
            }
            
            
            verificationCode = await GenerateVerificaionCode();           
            qrCodeUrl = $"https://www.torruna.store/Account/Create?tag=" + tagNo;
            byte[] qRCodeAsBytes = null;
            if (tagType==1)
            {
                qRCodeAsBytes = await GenerateQRCode(qrCodeUrl);
            }
            else if(tagType==2)
            {
                qRCodeAsBytes = await GenerateIdTag(qrCodeUrl,tagNo);
            }
            else
            {
                qRCodeAsBytes = null;
            }
            
            //string qrCodeAsImageBase64 = $"data:image/png;base64,{Convert.ToBase64String(qRCodeAsBytes)}";
            qrCodeImageUrl = qRCodeAsBytes;
            Tag tag = Tag.CreateTag(userId, tagNo, verificationCode, description, isVerification, isActive,qrCodeUrl,qrCodeImageUrl,tagImages,tagName,tagBreed,tagAge,tagTelephoneNumber,orderNo,orderName,orderTelephone,orderSKU, DateTime.Now,createdUserId,customerName,address,isLoss,dogLeash,tagType);
            await _context.Tags.AddAsync(tag);
            return tag;
        }
        public  bool IsNumberExist(string number)
        {
            var tag = _context.Tags.Any(x => x.No == number);
            return tag;
        }
        public async Task<string> GenerateVerificaionCode(int lenght = 8)
        {
            const string chars = "0123456789";
            var random = new Random();
            var code = new string(Enumerable.Repeat(chars,lenght)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return code;
        }

        public async Task<byte[]> GenerateQRCode(string text)
        {
         
            byte[] QRCode = new byte[0];
            if (!string.IsNullOrEmpty(text))
            {
                QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
                QRCodeData data = qRCodeGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.L);
                BitmapByteQRCode bitmap = new BitmapByteQRCode(data);
                QRCode = bitmap.GetGraphic(20,"#ffffff", "#000000");
            }
            return QRCode;
        }
        public async Task<byte[]> GenerateIdTag(string text,string tagNo)
        {

            byte[] QRCode = new byte[0];
            if (!string.IsNullOrEmpty(text))
            {
                QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
                QRCodeData data = qRCodeGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.L);
                BitmapByteQRCode bitmap = new BitmapByteQRCode(data);
                QRCode = bitmap.GetGraphic(20);

               
                using (Bitmap bitmapCreate = new Bitmap(230,270))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmapCreate))
                    {

                        graphics.Clear(Color.White);
                        //float x = 0; // Metinlerin başlangıç noktasını ayarla
                        //float yFirst = (bitmap.Height - graphics.MeasureString(firstText, font).Height * 2) / 2; // İlk metnin başlangıç noktasını ayarla
                        //float ySecond = yFirst + graphics.MeasureString(firstText, font).Height; // İkinci metnin başlangıç noktasını ayarla
                        //float width = bitmap.Width - qrCodeWidth - 5; // Metin alanının genişliğini ayarla
                        //float height = bitmap.Height; ; // Metin alanının yüksekliğini ayarla
                        //StringFormat format = new StringFormat();
                        //format.Alignment = StringAlignment.Center;
                        //format.LineAlignment = StringAlignment.Center;
                        //StringFormat formatScanMe = new StringFormat();
                        //format.Alignment = StringAlignment.Center;
                        //format.LineAlignment = StringAlignment.Center;

                        //SizeF textSize = graphics.MeasureString(firstText, font2);

                        //// Eğer metin kutusuna sığmıyorsa fontu küçült
                        //if (textSize.Width > width || textSize.Height > height)
                        //{
                        //    // Yeni font boyutunu belirle
                        //    float newFontSize = 55;

                        //    // Yeni boyut belirlenene kadar fontu küçült
                        //    while (textSize.Width > width || textSize.Height > height)
                        //    {
                        //        newFontSize -= 0.5f; // Yeni boyutu azalt
                        //        font2 = new Font(font.FontFamily, newFontSize, font2.Style);
                        //        textSize = graphics.MeasureString(firstText, font2);
                        //    }
                        //}

                        // İlk metni çizin
                        Font font2 = new Font("Arial", 25, FontStyle.Regular);// Yazı tipi ve boyutunu artır
                        Brush textBrush = Brushes.Black;
                        StringFormat format = new StringFormat();
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;
                        //graphics.DrawString(tagNo, font2, textBrush, new RectangleF(0, 20, 200, 100), format);
                        
                        // İkinci metni çizin


                        // Resmi sağ tarafa yerleştir
                        graphics.DrawImage((Image)new Bitmap(new MemoryStream(QRCode)),0, 0, 230,230);
                        graphics.DrawString(tagNo, font2, textBrush, new RectangleF(5,215,220,50), format);

                    }

                    // Resmi byte dizisine çevir ve döndür
                    using (MemoryStream ms = new MemoryStream())
                    {
                        
                        bitmapCreate.Save(ms, ImageFormat.Png);
                        return ms.ToArray();
                    }
                }
            }
            return null;
        }
        public async Task<string> GenerateTagNo(int length = 6)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            const string chars = "0123456789";
            char[] code = new char[length];

            for (int i = 0; i < length; i++)
            {
                code[i] = chars[random.Next(chars.Length)];
            }

            return new string(code);
        }

        public async Task<string> GetVerificationCodeByTag(string tag)
        {
            bool isTag= await _context.Tags.AnyAsync(x => x.No == tag);
            if(isTag)
            {
                var verificationCode= await _context.Tags.Where(x => x.No == tag).Select(x=>x.VerificationCode).FirstOrDefaultAsync();
                return verificationCode;
            }
            throw new Exception("Tag bulunamadı");

        }
        public async Task UpdateVerification(string tag, string userId)
        {
            var isTag = await _context.Tags.AnyAsync(x => x.No == tag);
            if(isTag)
            {
                var record = await _context.Tags.Where(x => x.No == tag).FirstOrDefaultAsync();
                if (record != null)
                {
                    record.UpdateVerification(userId);
                    _context.Tags.Update(record);
                    _context.SaveChanges();
                }               
            }
        }
        public async Task UpdateQrCodeImage(string tag, string userId)
        {
            var isTag = await _context.Tags.AnyAsync(x => x.No == tag);
            if (isTag)
            {
                var record = await _context.Tags.Where(x => x.No == tag).FirstOrDefaultAsync();
                if (record != null)
                {
                    record.UpdateQrCodeImage();
                    _context.Tags.Update(record);
                    _context.SaveChanges();
                }
            }
        }
        public async Task UpdateTagDetails(string tagNo, string tagName, string tagBreed, string tagAge, string tagImages, string description, string tagTelephoneNumber,bool isLoss)
        {
            var isTag = await _context.Tags.AnyAsync(x => x.No == tagNo);
            if (isTag)
            {
                var record = await _context.Tags.Where(x => x.No == tagNo).FirstOrDefaultAsync();
                if (record != null)
                {
                    record.UpdateTagDetails(description, tagImages, tagName, tagBreed, tagAge, tagTelephoneNumber,isLoss);
                    _context.Tags.Update(record);
                    _context.SaveChanges();
                }
            }
        }
        public async Task<bool> IsTagVerification(string tag)
        {
            var isTag = await _context.Tags.Where(x => x.No == tag).Select(x=>x.IsVerification).FirstOrDefaultAsync();
            return isTag;
        }
        public async Task<Tag> GetTagDetails(string tag)
        {
            var getTag = await _context.Tags.Where(x => x.No == tag).FirstOrDefaultAsync();
            return getTag;
            
        }

        public async Task<List<Tag>> GetUserTag(string userId)
        {
            if(userId!=null)
            {
                var userTag = await _context.Tags.Where(x => x.UserId == userId).ToListAsync();
                return userTag;                     
            }
            throw new Exception("Tag not found");
        }

        public async Task<List<View_TagUser>> GetTagList()
        {
            try
            {
                var tagList = await _context.View_TagUsers.ToListAsync();
                return tagList;
            }
            catch (Exception ex)
            {
                throw new Exception("Tag not found");
            }
        }

        public async Task CreateException(string exStackTrace, string exMessage,string sourceContext)
        {
            var errMessage = exStackTrace.Split("line");
            var lineNumber = "000";

            if (errMessage.Length > 1)
            {
                lineNumber = errMessage[1].Trim();
            }
            //return Task.CompletedTask;
            _logger.LogError($"Error Page:{sourceContext}, Error:{exMessage}, Error Line:{lineNumber}");
        }
    }
}
