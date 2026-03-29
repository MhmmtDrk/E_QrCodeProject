using MediatR;
using QrCode.Domain.AggregateModels.TagModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Exception.CreateException
{
    public sealed record CreateExceptionCommand(
        string SourceContext,
        string ExStackTrace,
        string ExMessage
        ) : IRequest;
}
