using System.ComponentModel.DataAnnotations;

namespace QrCode.WebUi.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
