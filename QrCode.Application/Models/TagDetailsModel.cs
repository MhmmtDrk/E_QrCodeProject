using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Models
{
    public class TagDetailsModel
    {       
        public string? No { get; private set; }        
        public string? Description { get; private set; }       
        public byte[]? TagImages { get; private set; }
        public string? TagName { get; set; }
        public string? TagBreed { get; set; }
        public string? TagAge { get; set; }
        public string? TagTelephoneNumber { get; set; }
    }
}
