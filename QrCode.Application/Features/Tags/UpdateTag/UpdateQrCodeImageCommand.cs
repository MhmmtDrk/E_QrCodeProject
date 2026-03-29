using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Tags.UpdateTag
{
    public class UpdateQrCodeImageCommand:IRequest
    {
        public string? Tag { get; set; }
        public string? UserId { get; set; }
    }
}
