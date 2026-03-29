using MediatR;
using QrCode.Domain.AggregateModels.Tags.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Tags.VerificationCode
{
    public class GetVerificationCodeHandler : IRequestHandler<GetVerificationCode, string>
    {
        private readonly ITagService _tagservice;
        public GetVerificationCodeHandler(ITagService tagservice)
        {
            _tagservice = tagservice;
        }
        public async Task<string> Handle(GetVerificationCode request, CancellationToken cancellationToken)
        {
            // Servisi kullanarak değeri al
            var value = await _tagservice.GetVerificationCodeByTag(request.tag);
            return value;
        }
    }
}
