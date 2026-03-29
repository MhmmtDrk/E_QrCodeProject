using MediatR;
using Microsoft.AspNetCore.Mvc;
using QrCode.Application.Features.Tags.UpdateTagDetails;
using QrCode.Application.Features.Tags.CreateTag;
using QrCode.Application.Features.Tags.GetTagDetails;
using QrCode.Application.Models;
using QrCode.Domain.AggregateModels.TagModels;
using static System.Net.Mime.MediaTypeNames;
using QrCode.WebUi.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using Microsoft.AspNetCore.Identity;
using QrCode.Domain.AggregateModels.Users;
using QrCode.Application.Features.Tags.GetUserTag;
using Microsoft.AspNetCore.Authorization;
using System.Drawing.Imaging;
using Image = System.Drawing.Image;
using QrCode.Domain.AggregateModels.Email.Events;
using System.Net;
using QrCode.Application.Features.Tags.GetTagList;
using QrCode.Domain.AggregateModels.Tags;
using OfficeOpenXml;
using Org.BouncyCastle.Ocsp;
using OfficeOpenXml.Style;
using NuGet.Protocol.Plugins;
using QrCode.Application.Features.Tags.UpdateTag;
using Microsoft.Extensions.Logging;
using QrCode.Application.Features.Exception.CreateException;

namespace QrCode.WebUi.Controllers
{
    [Authorize]
    public class TagController : Controller
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<TagController> _logger;

        public TagController(IMediator mediator, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment, ILogger<TagController> logger)
        {
            _mediator = mediator;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _logger= logger;
        }
        [HttpGet]
        public IActionResult CreateQrCode()
        {            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateQrCode(QrCodeModel model)
        {
            try
            {
                //string tagImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/no-image-icon.png");
                var user = await _userManager.GetUserAsync(User);
                //var tagImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", "images/no-image-icon.png");
                string tagImagePath = "/images/no-image-icon.png";
                // Resim dosyasını oku ve byte dizisine dönüştür
                //byte[] tagImageBytes = System.IO.File.ReadAllBytes(tagImagePath);
                var createTagCommand = new CreateTagCommand("", "", "", "", false, true, "", null, tagImagePath, "", "", "", "", model.OrderNo, model.OrderName, model.OrderTelephone, model.OrderSKU, DateTime.Now, user?.Id, model.CustomerName, model.Address, model.IsLoss, model.DogLeash, 1);
                var tag = await _mediator.Send(createTagCommand);

                model.TagNo = tag.No;
                string firstText = tag.OrderName;
                string secondText = tag.OrderTelephone;
                byte[] imageBytes = tag.QrCodeImageUrl; // Resmi byte dizisi olarak al

                // Metin ve resmi birleştir
                byte[] mergedImageBytes = MergeTextAndImage(firstText, secondText, imageBytes);
                model.QrCodeImage = mergedImageBytes;
                // Birleştirilmiş görüntüyü bir PNG olarak çıktı olarak al
                // İsteğe bağlı olarak byte dizisini döndürebiliriz

                var base64String = Convert.ToBase64String(model.QrCodeImage);
                TempData["QrCodeImageUrl"] = base64String;
                await _mediator.Send(new UpdateQrCodeImageCommand { Tag = tag.No, UserId = tag.UserId });
                //byte[] qRCodeAsBytes = _qrCodeGeneratorHelper.GenerateQRCode(text);
                //string qrCodeAsImageBase64 = $"data:image/png;base64,{Convert.ToBase64String(qRCodeAsBytes)}";
                //GenerateQrCodeViewModel model = new GenerateQrCodeViewModel();
                //model.QRCodeImageURL = qrCodeAsImageBase64;
            }
            catch (Exception ex)
            {
                await _mediator.Send(new CreateExceptionCommand("TagController/CreateQrCode", ex.StackTrace, ex.Message));
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult CreateIDTag()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateIDTag(int quantity)
        {

            //string folderPath = @"/IDTag";
            //byte[][] images = new byte[quantity][];
           
            try
            {
                List<(string tagNo, byte[] images)> tagList = new List<(string tag, byte[] images)>();

                if (quantity > 0)
                {
                    for (int i = 0; i < quantity; i++)
                    {
                        string tagImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/no-image-icon.png");
                        //string tagImagePath = "/images/no-image-icon.png";
                        var user = await _userManager.GetUserAsync(User);
                        // Resim dosyasını oku ve byte dizisine dönüştür
                        byte[] tagImageBytes = System.IO.File.ReadAllBytes(tagImagePath);
                        var createTagCommand = new CreateTagCommand("", "", "", "", false, true, "", null, "/images/no-image-icon.png", "", "", "", "", "", "", "", "", DateTime.Now, user?.Id, "", "", false, "", 2);
                        var tag = await _mediator.Send(createTagCommand);
                        //model.TagNo = tag.No;
                        //string firstText = tag.OrderName;
                        //string secondText = tag.OrderTelephone;
                        //byte[] imageBytes = tag.QrCodeImageUrl;// Resmi byte dizisi olarak al
                        tagList.Add((tag.No, tag.QrCodeImageUrl));
                        await _mediator.Send(new UpdateQrCodeImageCommand { Tag = tag.No, UserId = user.Id });

                        // Metin ve resmi birleştir
                        //byte[] mergedImageBytes = MergeTextAndImage(firstText, secondText, imageBytes);
                        //model.QrCodeImage = mergedImageBytes;
                        // Birleştirilmiş görüntüyü bir PNG olarak çıktı olarak al
                        // İsteğe bağlı olarak byte dizisini döndürebiliriz

                        //var base64String = Convert.ToBase64String(imageBytes);
                        //TempData["QrCodeImageUrl"] = base64String;
                    }
                    MemoryStream zipStream = new MemoryStream();

                    // Zip dosyası oluştur
                    using (var zipArchive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Create, true))
                    {

                        foreach (var tag in tagList)
                        {
                            var entry = zipArchive.CreateEntry($"{tag.tagNo}.png");
                            using (var entryStream = entry.Open())
                            {
                                await entryStream.WriteAsync(tag.images, 0, tag.images.Length);
                            }
                        }
                        //for (int i = 0; i < tagList.Count; i++)
                        //{
                        //    var entry = zipArchive.CreateEntry($"{i}.jpg");
                        //    using (var entryStream = entry.Open())
                        //    {
                        //        await entryStream.WriteAsync(images[i], 0, images[i].Length);
                        //    }
                        //}
                    }

                    // Stream'ı sıfırla ve zipStream'in pozisyonunu başa al
                    zipStream.Seek(0, SeekOrigin.Begin);

                    // İstemciye zip dosyasını indir
                    return File(zipStream, "application/zip", $"ID_Tags_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.zip");

                }
            }
            catch(Exception ex)
            {
                await _mediator.Send(new CreateExceptionCommand("TagController/CreateIDTag", ex.StackTrace,ex.Message));
            }
            //byte[] qRCodeAsBytes = _qrCodeGeneratorHelper.GenerateQRCode(text);
            //string qrCodeAsImageBase64 = $"data:image/png;base64,{Convert.ToBase64String(qRCodeAsBytes)}";
            //GenerateQrCodeViewModel model = new GenerateQrCodeViewModel();
            //model.QRCodeImageURL = qrCodeAsImageBase64;
            return View();
        }
        [HttpGet]
        public IActionResult CreatePNG()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePNG(CreatePNG model)
        {
            using (Bitmap bitmap = new Bitmap(700, 235))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    Color backgroundColor = Color.White;
                    graphics.Clear(backgroundColor);
                    // Resmi sol üst köşeye yerleştir
                    //graphics.DrawImage((Image)new Bitmap(new MemoryStream(qrCodeBytes)), 0, 0, qrCodeWidth, qrCodeHeight);

                    // İki metni tek bir metin kutusunda birleştir
                    string combinedText = $"{model.OrderName}\n{model.OrderTelephone}"; // İki metni bir satır boşluk bırakarak birleştir
                    
                    //// Metni ortala
                    //StringFormat format = new StringFormat();
                    //format.Alignment = StringAlignment.Center;
                    //format.LineAlignment = StringAlignment.Center;

                    //// Metni resmin yanına hizala
                    //float x = qrCodeWidth + 5; // Resmin sağ tarafından 10 piksel uzaklık
                    //float y = 0; // Resmin üstünde hizala
                    //float width = bitmap.Width - qrCodeWidth - 10; // Resmin solundaki alan
                    //float height = bitmap.Height; // Resmin yüksekliği
                    //RectangleF rect = new RectangleF(x, y, width, height);

                    //// Metni resmin yanına ekle
                    //graphics.DrawString(combinedText, font, textBrush, rect, format);
                    Font font = new Font("Arial", 65, FontStyle.Bold);
                    Font font2 = new Font("Arial", 65, FontStyle.Bold);// Yazı tipi ve boyutunu artır
                    Brush textBrush = Brushes.Black;
                    float x = 0; // Metinlerin başlangıç noktasını ayarla
                    float yFirst = (bitmap.Height - graphics.MeasureString(model.OrderName, font).Height * 2) / 2; // İlk metnin başlangıç noktasını ayarla
                    float ySecond = yFirst + graphics.MeasureString(model.OrderName, font).Height; // İkinci metnin başlangıç noktasını ayarla
                    float width = bitmap.Width; // Metin alanının genişliğini ayarla
                    float height = bitmap.Height; ; // Metin alanının yüksekliğini ayarla
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    StringFormat formatScanMe = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    SizeF textSize = graphics.MeasureString(model.OrderName, font2);

                    // Eğer metin kutusuna sığmıyorsa fontu küçült
                    if (textSize.Width > width || textSize.Height > height)
                    {
                        // Yeni font boyutunu belirle
                        float newFontSize = 60;

                        // Yeni boyut belirlenene kadar fontu küçült
                        while (textSize.Width > width || textSize.Height > height)
                        {
                            newFontSize -= 0.5f; // Yeni boyutu azalt
                            font2 = new Font(font.FontFamily, newFontSize, font2.Style);
                            textSize = graphics.MeasureString(model.OrderName, font2);
                        }
                    }

                    // İlk metni çizin
                    graphics.DrawString(model.OrderName, font2, textBrush, new RectangleF(x, 10, width, height / 2), format);
                    graphics.DrawString(model.OrderTelephone, font, textBrush, new RectangleF(x, 227 - height / 2 , width, height / 2), format);
                    // İkinci metni çizin


                    // Resmi sağ tarafa yerleştir

                }

                // Resmi byte dizisine çevir ve döndür
                using (MemoryStream ms = new MemoryStream())
                {
                    var user = await _userManager.GetUserAsync(User);
                    model.Address = model.Address == null ? "" : model.Address;
                    var createTagCommand = new CreateTagCommand("",  "", "", "", true, true, "",null, "", model.OrderName, "", "", model.OrderTelephone, model.OrderNo,model.OrderName, model.OrderTelephone, model.OrderSKU, DateTime.Now, user?.Id, model.CustomerName,model.Address, false, model.DogLeash, 3);
                    var tag = await _mediator.Send(createTagCommand);
                    bitmap.Save(ms, ImageFormat.Png);
                    var byteImage = ms.ToArray();
                    var base64String = Convert.ToBase64String(byteImage);
                    model.Image = base64String;
                    return View(model);
                }
            }
        }
        private byte[] MergeTextAndImage(string firstText,string secondText, byte[] qrCodeBytes)
        {
            try
            {
                // QR kodunun boyutunu yarısına indir
                int qrCodeWidth = 230;
                int qrCodeHeight = 230;

                // Metin boyutunu ayarla ve daha küçült
                Font font = new Font("Arial", 62, FontStyle.Bold);
                Font font2 = new Font("Arial", 62, FontStyle.Bold);// Yazı tipi ve boyutunu artır
                Font fontScanMe = new Font("Arial", 15, FontStyle.Regular);
                // Metin rengini ayarla
                Brush textBrush = Brushes.Black;
                Brush textBrushScanMe = Brushes.White;
                Color backgroundColor = Color.White;
                // Metin boyutunu ölç
                using (Bitmap tempBitmap = new Bitmap(1, 1))
                using (Graphics tempGraphics = Graphics.FromImage(tempBitmap))
                {


                    // Yeni metin boyutunu hesapla
                    SizeF adjustedFirstTextSize = tempGraphics.MeasureString(firstText, font);
                    SizeF secondTextSize = tempGraphics.MeasureString(secondText, font);

                    // Yeni QR kod yüksekliğini ayarla
                    qrCodeHeight = (int)Math.Max(qrCodeHeight, Math.Max(adjustedFirstTextSize.Height, secondTextSize.Height) * 2); // İki metni de tek bir satıra sığacak şekilde ayarla
                }

                // Resmi metinle aynı yükseklikte olacak şekilde küçült
                double resizeFactor = (double)qrCodeHeight / qrCodeWidth;
                qrCodeWidth = (int)Math.Max(qrCodeWidth * resizeFactor, 1);
                qrCodeHeight = (int)Math.Max(qrCodeHeight, 1);

                // Birleştirilmiş resmi oluştur
                using (Bitmap bitmap = new Bitmap(qrCodeWidth + 600, qrCodeHeight + 5))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.Clear(backgroundColor);
                        // Resmi sol üst köşeye yerleştir
                        //graphics.DrawImage((Image)new Bitmap(new MemoryStream(qrCodeBytes)), 0, 0, qrCodeWidth, qrCodeHeight);

                        // İki metni tek bir metin kutusunda birleştir
                        string combinedText = $"{firstText}\n{secondText}"; // İki metni bir satır boşluk bırakarak birleştir

                        //// Metni ortala
                        //StringFormat format = new StringFormat();
                        //format.Alignment = StringAlignment.Center;
                        //format.LineAlignment = StringAlignment.Center;

                        //// Metni resmin yanına hizala
                        //float x = qrCodeWidth + 5; // Resmin sağ tarafından 10 piksel uzaklık
                        //float y = 0; // Resmin üstünde hizala
                        //float width = bitmap.Width - qrCodeWidth - 10; // Resmin solundaki alan
                        //float height = bitmap.Height; // Resmin yüksekliği
                        //RectangleF rect = new RectangleF(x, y, width, height);

                        //// Metni resmin yanına ekle
                        //graphics.DrawString(combinedText, font, textBrush, rect, format);

                        float x = 0; // Metinlerin başlangıç noktasını ayarla
                        float yFirst = (bitmap.Height - graphics.MeasureString(firstText, font).Height * 2) / 2; // İlk metnin başlangıç noktasını ayarla
                        float ySecond = yFirst + graphics.MeasureString(firstText, font).Height; // İkinci metnin başlangıç noktasını ayarla
                        float width = bitmap.Width - qrCodeWidth; // Metin alanının genişliğini ayarla
                        float height = bitmap.Height; ; // Metin alanının yüksekliğini ayarla
                        StringFormat format = new StringFormat();
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;
                        StringFormat formatScanMe = new StringFormat();
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;

                        SizeF textSize = graphics.MeasureString(firstText, font2);

                        // Eğer metin kutusuna sığmıyorsa fontu küçült
                        if (textSize.Width > width || textSize.Height > height)
                        {
                            // Yeni font boyutunu belirle
                            float newFontSize = 60;

                            // Yeni boyut belirlenene kadar fontu küçült
                            while (textSize.Width > width || textSize.Height > height)
                            {
                                newFontSize -= 0.5f; // Yeni boyutu azalt
                                font2 = new Font(font.FontFamily, newFontSize, font2.Style);
                                textSize = graphics.MeasureString(firstText, font2);
                            }
                        }

                        // İlk metni çizin
                        graphics.DrawString(firstText, font2, textBrush, new RectangleF(x, 10, width, height / 2), format);
                        graphics.DrawString(secondText, font, textBrush, new RectangleF(x, 227- height / 2, width, height / 2), format);
                        // İkinci metni çizin


                        // Resmi sağ tarafa yerleştir
                        graphics.DrawImage((Image)new Bitmap(new MemoryStream(qrCodeBytes)), width + 5,0, qrCodeWidth, qrCodeHeight+5);
                        graphics.DrawString("CALL MY PEOPLE", fontScanMe, textBrushScanMe, new RectangleF(600, -10
                            , qrCodeWidth, 50), format);
                        graphics.DrawString("SCAN ME", fontScanMe, textBrushScanMe, new RectangleF(600, 196, qrCodeWidth, 50), format);
                    }

                    // Resmi byte dizisine çevir ve döndür
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bitmap.Save(ms, ImageFormat.Png);
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                _mediator.Send(new CreateExceptionCommand("TagController/MergeTextAndImage", ex.StackTrace, ex.Message));
                throw new Exception();
            }

        }

       
        public async Task<IActionResult> GetUserTag()
        {
            var user = await _userManager.GetUserAsync(User);
            var userTag=await _mediator.Send(new GetUserTagCommand(user.Id));    
            return View(userTag);
        }
        public async Task<IActionResult> UpdateTagDetails(string tag)
        {
            
            if (!string.IsNullOrEmpty(tag))
            {
                var tagDetails = await _mediator.Send(new GetTagDetailsCommand(tag));


                var updateTag = new TagUpdateModel
                {
                    TagNo = tagDetails.No,
                    TagAge = tagDetails.TagAge,
                    TagBreed = tagDetails.TagBreed,
                    Description = tagDetails.Description,
                    TagImages = null,
                    TagName = tagDetails.TagName,
                    TagTelephoneNumber = tagDetails.TagTelephoneNumber,
                    IsLoss = tagDetails.IsLoss,
                    TagImagesText = tagDetails.TagImages
                };
                

                //TempData["DetailTag"] = tag;
                return View(updateTag);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTagDetails(TagUpdateModel updateRequest)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    string imageData = "";
                    if (updateRequest.TagImages == null || updateRequest.TagImages.Length == 0)
                    {
                        imageData = "/images/no-image-icon.png";
                        var tagDetails = await _mediator.Send(new GetTagDetailsCommand(updateRequest.TagNo));
                        if (tagDetails != null)
                        {
                            //imageData= Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", "no-image-icon.png");
                            var filePathStr = tagDetails.TagImages;
                            imageData = filePathStr;
                        }
                    }
                    else
                    {
                        // var fileName = Path.GetFileName(updateRequest.TagImages.FileName);
                        var fileExtension = Path.GetExtension(updateRequest.TagImages.FileName);
                        var fileName = updateRequest.TagName + "_" + updateRequest.TagNo + fileExtension;
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                        var filePathStr = "/uploads/" + fileName;
                        imageData = filePathStr;
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await updateRequest.TagImages.CopyToAsync(stream);
                        }
                        //using (var memoryStream = new MemoryStream())
                        //{
                        //    updateRequest.TagImages.CopyTo(memoryStream);
                        //    imageData = memoryStream.ToArray();

                        //}
                    }
                    var user = await _userManager.GetUserAsync(User);
                    var tagUpdateCommand = new UpdateTagDetailsCommand(updateRequest.TagNo, updateRequest.TagName, updateRequest.TagBreed, updateRequest.TagAge, imageData, updateRequest.Description, updateRequest.TagTelephoneNumber,
                        updateRequest.IsLoss);

                    await _mediator.Send(tagUpdateCommand);
                    return RedirectToAction("GetTagDetails", "Tag", new { tag = tagUpdateCommand.Tag });
                }
            }
            catch (Exception ex)
            {
                var user = await _userManager.GetUserAsync(User);
                await _mediator.Send(new CreateExceptionCommand("TagController/UpdateTagDetails", ex.StackTrace, ex.Message));
                var tagUpdateCommand = new UpdateTagDetailsCommand(updateRequest.TagNo, updateRequest.TagName, ex.Message, updateRequest.TagAge, "", updateRequest.Description, updateRequest.TagTelephoneNumber,
                        updateRequest.IsLoss);

                await _mediator.Send(tagUpdateCommand);
            }
            return View(updateRequest);
        }
        [AllowAnonymous]
        public async Task<IActionResult> GetTagDetails(string tag)
        {
            if (!string.IsNullOrEmpty(tag))
            {
                var tagDetails = await _mediator.Send(new GetTagDetailsCommand(tag));
                if(tagDetails.IsVerification == false)
                {
                    return RedirectToAction("Error", "Home");
                }
                //var imagesList = tagDetails.TagImages.Split(',').ToList();
                var TagDetailsModel = new TagDetailModel
                {
                    TagImages = tagDetails.TagImages,
                    TagName = tagDetails.TagName,
                    Description = tagDetails.Description,
                    TagAge = tagDetails.TagAge,
                    TagBreed = tagDetails.TagBreed,
                    TagNo = tagDetails.No,
                    TagTelephoneNumber = tagDetails.TagTelephoneNumber,
                    IsLoss = tagDetails.IsLoss
                };
                TempData["GoogleMapsTagNo"] = tagDetails.No;
                return View(TagDetailsModel);
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetGoogleMapsUrl(string latitude, string longitude)
        {
            try
            {
                string googleMapsURL = $"https://www.google.com/maps?q={latitude},{longitude}";
                string tag = TempData["GoogleMapsTagNo"].ToString();
                if (!string.IsNullOrEmpty(tag))
                {
                    var tagDetails = await _mediator.Send(new GetTagDetailsCommand(tag));
                    if (tagDetails.IsLoss == true)
                    {
                        var user = await _userManager.FindByIdAsync(tagDetails.UserId);
                        string body = $@"
                <html>                   
                    <body>
                        <table width=""600""  border=""0"" cellpadding=""0"" cellspacing=""0"" style=""padding:5;background-color:#ffffff"">
                            <tbody>
                                <tr>
                                    <td style=""font-size:14px;padding:0 20px 8px 30px;width:600px;font-family:Arial,Regular;text-align:center;background-color:#ffffff;border-right:1px #ebebf0 solid;border-left:1px #ebebf0 solid;line-height:20px"" valign=""top"">
                                    <a href='https://www.torruna.store' target='_blank' style='font-size: x-large;color: #ED6436;text-decoration: none;text-align:center;'><img src='https://torruna.store/images/torruna.png' style=""width:40px""/><br><span style='font-color:#ed6436'> Torruna</span></a>
                                    <hr></hr>
                                    </td>
                                </tr>  
                                <tr>
                                    <td style=""font-size:14px;padding:0 20px 8px 30px;width:600px;font-family:Arial,Regular;text-align:justify;background-color:#ffffff;border-right:1px #ebebf0 solid;border-left:1px #ebebf0 solid;line-height:20px"" valign=""top"">
                                        The QR code on your pet's collar has been scanned. The person who found your pet would be reaching out to you shortly. To view the current location, please click on the link below.</td>
                                </tr>  
                                <tr>
                                    <td style=""font-size:14px;padding:0 20px 8px 30px;width:600px;font-family:Arial,Regular;text-align:justify;background-color:#ffffff;border-right:1px #ebebf0 solid;border-left:1px #ebebf0 solid;line-height:20px"" valign=""top"">
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""font-size:14px;padding:0 20px 8px 30px;width:600px;font-family:Arial,Regular;background-color:#ffffff;border-right:1px #ebebf0 solid;border-left:1px #ebebf0 solid;line-height:20px;text-align:center"" valign=""top"">
                                        <a href='https://www.google.com/maps?q={latitude},{longitude}' target='_blank' style='font-size: x-large;color: #ED6436;text-decoration: none;text-align:center;display: ruby;'><span style='font-color:#65C178'>Click to see the location</span> <br><img src='https://torruna.store/images/icons8-location.gif' style=""width:15%""/></a>
                                    </td>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </body>
                </html>";
                        //var body = $"<p>The QR code on your pet's collar has been scanned. The person who found your pet would be reaching out to you shortly. To view the current location, please click on the link below. <br><br><a href='https://www.google.com/maps?q={latitude},{longitude}' target='_blank'><i class='fas fa-map-marked-alt'></i> Click to see the location</a></p>";
                        await _mediator.Publish(new EmailSentEvent(user.Email, $"{tagDetails.OrderName}'s Pet Tag was scanned!", body));
                    }
                }
                return Json(new { success = true, message = "Mail send" });
            }
            catch (Exception ex)
            {
                await _mediator.Send(new CreateExceptionCommand("TagController/GetGoogleMapsUrl", ex.StackTrace, ex.Message));
                throw new Exception();
            }
        }
        public IActionResult Download()
        {
            // Önceden oluşturulmuş bir byte dizisi
            return View();
        }
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetTagList(string searchString, DateTime startDate, DateTime endDate,int tagType, int page = 1, int pageSize = 10)
        {
            if(startDate.ToString().Contains("1.01.0001"))
            {
                startDate = DateTime.Now; endDate=DateTime.Now;
                DateTime modifiedDateTimeStart = startDate.Date // Gelen tarihi al
            .AddHours(0) // Saat bilgisini değiştir
            .AddMinutes(0) // Dakika bilgisini değiştir
            .AddSeconds(0);
                startDate = modifiedDateTimeStart;
                // startDate.Hour = 0;
            }
            var tagList = await _mediator.Send(new GetTagListCommand());
            DateTime modifiedDateTimeEnd = endDate.Date // Gelen tarihi al
            .AddHours(23) // Saat bilgisini değiştir
            .AddMinutes(59) // Dakika bilgisini değiştir
            .AddSeconds(59);
            endDate = modifiedDateTimeEnd;
            if (!string.IsNullOrEmpty(searchString))
            {
                tagList = tagList.Where(t => t.OrderName.ToLower().Contains(searchString.ToLower()) || t.OrderNo.ToLower().Contains(searchString.ToLower()) || t.Address.ToLower().Contains(searchString.ToLower()) && (t.CreatedDate >= startDate && t.CreatedDate < endDate) && t.TagType==tagType).ToList();
            }
            else
            {
                tagList = tagList.Where(t => t.CreatedDate >= startDate && t.CreatedDate < endDate && t.TagType == tagType).ToList();
            }
            ViewData["StartDate"] = startDate.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate.ToString("yyyy-MM-dd");
            ViewData["SearchString"] = searchString;
            ViewData["TagType"] = tagType;
            var paginatedTags = tagList.OrderBy(x=>x.CreatedDate)
                .Skip((page - 1) * pageSize)
                //.Take(pageSize)
                .ToList();
            return View(paginatedTags);
        }
        public async Task<IActionResult> ExportToExcel(string searchString, DateTime startDate, DateTime endDate,int tagType)
        {
            if (startDate.ToString().Contains("1.01.0001"))
            {
                startDate = DateTime.Now; endDate = DateTime.Now;
            }
            var tagList = await _mediator.Send(new GetTagListCommand());
            DateTime modifiedDateTime = endDate.Date // Gelen tarihi al
            .AddHours(23) // Saat bilgisini değiştir
            .AddMinutes(59) // Dakika bilgisini değiştir
            .AddSeconds(59);
            endDate = modifiedDateTime;
            if (!string.IsNullOrEmpty(searchString))
            {
                tagList = tagList.Where(t => t.OrderName.ToLower().Contains(searchString.ToLower()) || t.OrderNo.ToLower().Contains(searchString.ToLower()) || t.Address.ToLower().Contains(searchString.ToLower()) && (t.CreatedDate >= startDate && t.CreatedDate < endDate) && t.TagType == tagType).ToList();
            }
            else
            {
                tagList = tagList.Where(t => t.CreatedDate >= startDate && t.CreatedDate < endDate && t.TagType == tagType).ToList();
            }
            var data = tagList.OrderBy(x=>x.CreatedDate); // Verileri almak için bir metot
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                
                var worksheet = package.Workbook.Worksheets.Add("Data");
                worksheet.Cells[1, 1].Value = "Tag ID";
                worksheet.Cells[1, 2].Value = "Order ID";
                worksheet.Cells[1, 3].Value = "Customer Name and Address";
                worksheet.Cells[1, 4].Value = "SKU";
                worksheet.Cells[1, 5].Value = "Dog's Name";
                worksheet.Cells[1, 6].Value = "Phone Number";
                worksheet.Cells[1, 7].Value = "Leather Dog Leash";
                worksheet.Cells[1, 8].Value = "Created Date";
                
                int rowIndex = 2;
                for (int i = 1; i < 9; i++)
                {
                    worksheet.Cells[1, i].Style.Font.Bold = true;
                    worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid; // Dolgu deseni belirleme
                    worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    worksheet.Cells[1, i].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Column(i).AutoFit();
                }
                foreach (var item in data)
                {
                    // Örnek olarak, item.Date bir DateTime türünde olan tarih değerini temsil eder
                    // Örnek olarak, tarih sütunu (1. sütun)
                    worksheet.Cells[rowIndex, 8].Style.Numberformat.Format= "dd/MM/yyyy HH:mm:ss";
                    worksheet.Cells[rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[rowIndex, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[rowIndex, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[rowIndex, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[rowIndex, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[rowIndex, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[rowIndex, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment=ExcelHorizontalAlignment.Left;
                    worksheet.Cells[rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    //worksheet.Cells[rowIndex, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);


                    worksheet.Cells[rowIndex, 1].Value = item.No;
                    worksheet.Cells[rowIndex, 2].Value = item.OrderNo;
                    worksheet.Cells[rowIndex, 3].Value = item.Address;
                    worksheet.Cells[rowIndex, 4].Value = item.OrderSKU;
                    worksheet.Cells[rowIndex, 5].Value = item.OrderName;
                    worksheet.Cells[rowIndex, 6].Value = item.OrderTelephone;
                    worksheet.Cells[rowIndex, 7].Value = item.DogLeash;//dog leash
                    worksheet.Cells[rowIndex, 8].Value = item.CreatedDate;
                    //worksheet.Cells[rowIndex, 9].Value = item.CustomerName;
                    rowIndex++;
                }
                for (int i = 1; i < 10; i++)
                {                 
                    worksheet.Column(i).AutoFit();
                }
                //worksheet.Cells.LoadFromCollection(data, true);
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"data-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

    }
}
