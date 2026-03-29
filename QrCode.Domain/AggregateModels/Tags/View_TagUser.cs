using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Domain.AggregateModels.Tags
{
    public class View_TagUser
    {
        public Guid Id { get; set; }
        public string? No { get; set; }        
        public string? OrderNo { get; set; }
        public string? OrderName { get; set; }
        public string? OrderTelephone { get; set; }
        public string? OrderSKU { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? CustomerName { get; set; }
        public bool IsLoss { get; set; }
        public string? DogLeash { get; set; }
        public int TagType { get; set; }
        public string TagImages { get; set; }
        public bool IsVerification { get; set; }
    }
}
