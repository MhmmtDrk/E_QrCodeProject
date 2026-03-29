using MediatR;
using QrCode.Domain.AggregateModels.TagModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Domain.AggregateModels.Tags.Events
{
    public class TagDomainEvent:INotification
    {
        public Tag Tag { get; }
        public TagDomainEvent(Tag tag)
        {
            Tag = tag;
        }
    }
}
