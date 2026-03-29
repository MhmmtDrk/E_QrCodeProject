using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Domain.AggregateModels.Tags.Events
{
    public  sealed class SendTagActivationCodeEmailEvent : INotificationHandler<TagDomainEvent>
    {
        public Task Handle(TagDomainEvent notification, CancellationToken cancellationToken)
        {
            //TagActivationMail
            return Task.CompletedTask;
        }
    }
}
