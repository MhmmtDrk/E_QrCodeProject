namespace QrCode.WebUi.Models
{
    public class QrCodeModel
    {
        public string? TagNo { get; set; } = "";
        public string? OrderNo { get; set; } = "";
        public string? OrderName { get; set; } = "";
        public string? OrderTelephone { get; set; } = "";
        public string? OrderSKU { get; set; } = "";
        public byte[]? QrCodeImage { get; set; }
        public string? CustomerName { get; set; } = "";
        public string? Address { get; set; } = "";
        public bool IsLoss { get; set; }
        public string? DogLeash { get; set; } = "";
    }
}
