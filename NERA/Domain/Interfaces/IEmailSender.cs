using Domain.Entities;

public interface IEmailSender
{
    Task SendEventRegistrationEmailAsync(
        string toEmail,
        string toName,
        Event ev,
        byte[] icsAttachment,
        byte[] qrCode);
    Task SendEventDeletedEmailAsync(
        string toEmail, 
        string toName, 
        Event ev);
    Task SendEventEditedEmailAsync(string toEmail,
        string toName,
        Event oldEvent,
        Event newEvent);
}
