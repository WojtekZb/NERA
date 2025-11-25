using Domain.Configuration;
using Domain.Entities;
using Domain.Interfaces;
using Logic.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Utils;

namespace Logic.SimpleMailTransferProtocol
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpSettings _settings;
        private readonly QrCodeGeneratorService _qrCodeGen;
        public SmtpEmailSender(SmtpSettings settings)
        {
            _settings = settings;
        }
        public async Task SendEventRegistrationEmailAsync(string toEmail, string toName, Event ev, byte[] icsAttachment, byte[] qrCode)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = $"Registration confirmed: {ev.Title}";

            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"Hi {toName},\n\nYou are registered for {ev.Title}.\n" +
                           $"When: {ev.StartDate:u} - {ev.EndDate:u} (UTC)\n\n" +
                           "The event is attached as a calendar file.\n\n" +
                           "Your QR code for check-in is included below."
            };

            // Add inline QR code
            var qrImage = bodyBuilder.LinkedResources.Add("event_qr.png", new MemoryStream(qrCode));
            qrImage.ContentId = MimeUtils.GenerateMessageId();

            bodyBuilder.HtmlBody =
                $"<p>Hi {toName},</p>" +
                $"<p>You are registered for <b>{ev.Title}</b>.</p>" +
                $"<p>When: {ev.StartDate:u} - {ev.EndDate:u} (UTC)</p>" +
                "<p>The event is attached as a calendar file.</p>" +
                "<p>Your QR code for check-in:</p>" +
                $"<p><img src=\"cid:{qrImage.ContentId}\" alt=\"Event QR Code\" /></p>";

            // Attach ICS file
            var icsPart = new MimePart("text", "calendar")
            {
                Content = new MimeContent(new MemoryStream(icsAttachment)),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = "event.ics"
            };
            icsPart.ContentType.Parameters["method"] = "REQUEST";
            icsPart.ContentType.Parameters["name"] = "event.ics";

            // Combine body + ICS
            var multipart = new Multipart("mixed")
        {
            bodyBuilder.ToMessageBody(),
            icsPart
        };
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
            try
            {
                const string host = "smtp.gmail.com";
                const int port = 587;

                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_settings.UserName, _settings.Password);
                await client.SendAsync(message);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }

    }
}
