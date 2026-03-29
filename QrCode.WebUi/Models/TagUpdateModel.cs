using System.ComponentModel.DataAnnotations;

namespace QrCode.WebUi.Models
{
    public class TagUpdateModel
    {

        public string? TagNo { get; set; }
        //[Required(ErrorMessage = "Tag Name is required.")]
        public string? TagName { get; set; }
        public string? TagBreed { get; set; }

        //[Required(ErrorMessage = "Tag Age is required.")]
        public string? TagAge { get; set; }
        public IFormFile? TagImages { get; set; }
        public string? TagImagesText { get; set; }

        //[Required(ErrorMessage = "Description is required.")]
        public string? Description { get; set; }
        //[Required(ErrorMessage = "Tag Telephone Number is required.")]
        public string? TagTelephoneNumber { get; set; }

        public bool IsLoss { get; set; }
    }
}
