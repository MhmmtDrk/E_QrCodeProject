using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Http;
using QrCode.Application.Models;
using QrCode.Domain.AggregateModels.TagModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Tags.UpdateTagDetails
{
    public sealed record UpdateTagDetailsCommand(        
        string? Tag,
        string? TagName,
        string? TagBreed,
        string? TagAge,
        string? TagImages,
        string? Description,
        string? TagTelephoneNumber,
        bool IsLoss):IRequest;
    
}
