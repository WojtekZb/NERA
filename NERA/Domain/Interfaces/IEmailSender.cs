using Domain.Entities;

public interface IEmailSender
{
    Task SendEventRegistrationEmailAsync(
        string toEmail,
        string toName,
        Event ev,
        byte[] icsAttachment);

    Task SendEventNotificationEmailAsync(
        string toEmail,
        string toName,
        string eventName,
        string action);
}
