using Domain.Entities;

public interface IEmailSender
{
    Task SendEventRegistrationEmailAsync(
        string toEmail,
        string toName,
        Event ev,
        byte[] icsAttachment,
        byte[] qrCode);
    }
}
