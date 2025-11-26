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

        public async Task SendEventDeletedEmailAsync(string toEmail, string toName, Event ev)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = $"Event Deleted: {ev.Title}";

            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"Hi {toName},\n\n" +
                           $"The event '{ev.Title}' has been deleted.\n" +
                           $"Date of deletion: {DateTime.UtcNow:u} (UTC)\n\n" +
                           "If this was unexpected, please contact support."
            };

            message.Body = bodyBuilder.ToMessageBody();
            await SendAsync(message);
        }

        public async Task SendEventEditedEmailAsync(string toEmail, string toName, Event oldEvent, Event newEvent)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = $"Event Edited: {newEvent.Title}";

            var changes = new List<string>();

            if (oldEvent.Title != newEvent.Title)
                changes.Add($"Title changed from '{oldEvent.Title}' to '{newEvent.Title}'");

            if (oldEvent.StartDate != newEvent.StartDate)
                changes.Add($"StartDate changed from {oldEvent.StartDate:u} to {newEvent.StartDate:u}");

            if (oldEvent.EndDate != newEvent.EndDate)
                changes.Add($"EndDate changed from {oldEvent.EndDate:u} to {newEvent.EndDate:u}");

            if (oldEvent.CGI != newEvent.CGI)
                changes.Add($"CGI changed from '{oldEvent.CGI}' to '{newEvent.CGI}'");

            if (oldEvent.Adress != newEvent.Adress)
                changes.Add($"Address changed from '{oldEvent.Adress}' to '{newEvent.Adress}'");

            if (oldEvent.Cost != newEvent.Cost)
                changes.Add($"Cost changed from {oldEvent.Cost} to {newEvent.Cost}");

            if (oldEvent.Capacity != newEvent.Capacity)
                changes.Add($"Capacity changed from {oldEvent.Capacity} to {newEvent.Capacity}");

            if (oldEvent.Description != newEvent.Description)
                changes.Add($"Description changed from '{oldEvent.Description}' to '{newEvent.Description}'");

            if (oldEvent.Status != newEvent.Status)
                changes.Add($"Status changed from '{oldEvent.Status}' to '{newEvent.Status}'");

            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"Hi {toName},\n\n" +
                           $"The event '{oldEvent.Title}' has been edited.\n\n" +
                           (changes.Count > 0
                               ? "Here are the changes:\n- " + string.Join("\n- ", changes)
                               : "No changes detected.") +
                           $"\n\nDate of edit: {DateTime.UtcNow:u} (UTC)\n\n" +
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
