using MediatR;
using QrCode.Domain.AggregateModels.Tags.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Domain.AggregateModels.Email.Events
{

    public class EmailSentEventHandler: INotificationHandler<EmailSentEvent>
    {
        private readonly IEmailService _emailService;

        public EmailSentEventHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public async Task Handle(EmailSentEvent notification, CancellationToken cancellationToken)
        {
            await _emailService.SendEmailAsync(notification.ToEmail, notification.Subject, notification.Body);
        }
    }
}
