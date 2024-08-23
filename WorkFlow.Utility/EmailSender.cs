using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Threading.Tasks;

namespace WorkFlow.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress("", email)); // Empty name for recipient
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, true); // Use SSL
            await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.SenderPassword); // Authenticate

            await client.SendAsync(message);
            await client.DisconnectAsync(true); // Disconnect
        }
    }
}
