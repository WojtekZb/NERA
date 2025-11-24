using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IEmailSender
    {
     Task SendEventRegistrationEmailAsync(
        string toEmail,
        string toName,
        Event ev,
        byte[] icsAttachment);
    }
}
