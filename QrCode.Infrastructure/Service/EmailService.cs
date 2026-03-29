using QrCode.Domain.AggregateModels.Email;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Net.Mail;
//using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using MimeKit;
using static QRCoder.PayloadGenerator;
using MailKit.Net.Smtp;
using MailKit.Security;
using QrCode.Application.Features.Exception.CreateException;

namespace QrCode.Infrastructure.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        //private readonly string _senderPassword;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var SenderEmail = _configuration.GetSection(nameof(EmailSettings))["SenderEmail"];
                var SenderPassword = _configuration.GetSection(nameof(EmailSettings))["SenderPassword"];
                //var fromAddress = new MailAddress(SenderEmail,"Torruna");

                var emailMessage = new MimeMessage();


                emailMessage.From.Add(new MailboxAddress("Torruna", SenderEmail));
                emailMessage.To.Add(new MailboxAddress("Torruna", toEmail));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart("html") { Text = body };


                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtpout.secureserver.net", 587, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(SenderEmail, SenderPassword);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Tag not found");
            }
            //using (var client = new SmtpClient())
            //{
            //    client.Host = "smtpout.secureserver.net";
            //    client.Port = 587; // Godaddy SMTP port
            //    client.EnableSsl = true;
            //    client.UseDefaultCredentials = false;
            //    client.Credentials = new NetworkCredential(fromAddress.Address, SenderPassword);
              
            //    // Create and configure the email message
            //    using (var message = new MailMessage(SenderEmail, toEmail))
            //    {
            //        message.Subject = subject;
            //        message.Body = body;
            //        message.IsBodyHtml = true;                    
            //        message.From = fromAddress;
              

            //        try
            //        {
            //            // Send the email
            //            await client.SendMailAsync(message);
            //            client.Dispose();
            //        }
            //        catch (Exception ex)
            //        {
            //            // Handle exceptions
            //            Console.WriteLine($"Failed to send email: {ex.Message}");
            //            throw; // Rethrow the exception
            //        }
            //    }
            //}
        }
    }
}
