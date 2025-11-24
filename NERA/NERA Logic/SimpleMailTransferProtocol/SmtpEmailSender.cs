
using Domain.Entities;
using Domain.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using Domain.Configuration;

namespace Logic.SimpleMailTransferProtocol
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpSettings _settings;

        public SmtpEmailSender(SmtpSettings settings)
        {
            _settings = settings;
        }

        public async Task SendEventRegistrationEmailAsync(string toEmail, string toName, Event ev, byte[] icsAttachment)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = $"Registration confirmed: {ev.Title}";

            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"Hi {toName},\n\nYou are registered for {ev.Title}.\n" +
                           $"When: {ev.StartDate:u} - {ev.EndDate:u} (UTC)\n\n" +
                           "The event is attached as a calendar file."
            };

            var icsPart = new MimePart("text", "calendar")
            {
                Content = new MimeContent(new MemoryStream(icsAttachment)),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = "event.ics"
            };

            var multipart = new Multipart("mixed") { bodyBuilder.ToMessageBody(), icsPart };
            message.Body = multipart;

            await SendAsync(message);
        }
        // Sends an email notification about an event action (updated or deleted)
        public async Task SendEventNotificationEmailAsync(string toEmail, string toName, string eventName, string action)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = $"Event {action}: {eventName}";

            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"Hi {toName},\n\nThe event '{eventName}' has been {action}.\n" +
                           $"Date and time of action: {DateTime.UtcNow:u} (UTC)\n\n" +
                           "If you have any questions, please contact support."
            };

            message.Body = bodyBuilder.ToMessageBody();

            await SendAsync(message);
        }

        private async Task SendAsync(MimeMessage message)
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port, _settings.UseSsl);
            await client.AuthenticateAsync(_settings.UserName, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
