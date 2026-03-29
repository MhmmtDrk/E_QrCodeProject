using MediatR;
using QrCode.Application.Features.Tags.CreateTag;
using QrCode.Domain.AggregateModels.TagModels;
using QrCode.Domain.AggregateModels.Tags.Events;
using QrCode.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Exception.CreateException
{
    internal class CreateExceptionCommandHandler: IRequestHandler<CreateExceptionCommand>
    {
        private readonly ITagService _tagService;
        private readonly IUnitOfWork _UnitOfWork;
        public CreateExceptionCommandHandler(ITagService tagService, IUnitOfWork unitOfWork)
        {
            this._tagService = tagService;
            this._UnitOfWork = unitOfWork;
        }

        public async Task Handle(CreateExceptionCommand request, CancellationToken cancellationToken)
        {
            //business kontrolleri yapabilirsin
            await _tagService.CreateException(request.ExStackTrace,request.ExMessage,request.SourceContext);           
        }
    }
}
