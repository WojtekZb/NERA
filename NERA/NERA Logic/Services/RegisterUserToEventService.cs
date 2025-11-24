using Domain.Entities;
using Domain.Interfaces;
using System.Text;
namespace Logic.Services
{
    public class RegisterUserToEventService
    {
        private readonly IRegisterUserToEventRepo _repo;
        private readonly IEmailSender _emailSender;
        private readonly ICreateEventRepo _createEventRepo;

        public RegisterUserToEventService(IRegisterUserToEventRepo repo, IEmailSender emailSender, ICreateEventRepo createEventRepo)
        {
            _repo = repo;
            _emailSender = emailSender;
            _createEventRepo = createEventRepo;
        }

        public async Task RegisterForEvent(string UserSub, string userEmail, string userName, int EventId)
        {
            await _repo.RegisterUserAsync(UserSub, EventId);

            var ev = await _createEventRepo.GetByIdAsync(EventId)
         ?? throw new InvalidOperationException("Event not found");

            // 3. Build .ics bytes
            var icsBytes = BuildIcs(ev, userEmail, userName);

            // 4. Send email with attachment
            await _emailSender.SendEventRegistrationEmailAsync(userEmail, userName, ev, icsBytes);
        }

        private byte[] BuildIcs(Event ev, string attendeeEmail, string attendeeName)
        {
            // ICS requires UTC in this format: yyyyMMdd'T'HHmmss'Z'
            string FormatUtc(DateTime dt) => dt.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'");

            var sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("PRODID:-//YourApp//EN");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("METHOD:REQUEST");

            sb.AppendLine("BEGIN:VEVENT");

            // Use the event's own UID so updates stay consistent
            sb.AppendLine($"UID:{ev.Id}");

            // Sequence = 0 for first request
            sb.AppendLine("SEQUENCE:0");

            sb.AppendLine($"DTSTAMP:{FormatUtc(DateTime.UtcNow)}");
            sb.AppendLine($"DTSTART:{FormatUtc(ev.StartDate)}");
            sb.AppendLine($"DTEND:{FormatUtc(ev.EndDate)}");

            sb.AppendLine($"SUMMARY:{Escape(ev.Title)}");

            if (!string.IsNullOrWhiteSpace(ev.Description))
                sb.AppendLine($"DESCRIPTION:{Escape(ev.Description!)}");

            if (!string.IsNullOrWhiteSpace(ev.Adress))
                sb.AppendLine($"LOCATION:{Escape(ev.Adress!)}");

            // Organizer is recommended for REQUESTs
            //sb.AppendLine($"ORGANIZER;CN=\"{Escape(ev.OrganizerName)}\":mailto:{ev.OrganizerEmail}");

            // Add attendee
            sb.AppendLine($"ATTENDEE;CN=\"{Escape(attendeeName)}\";RSVP=TRUE:mailto:{attendeeEmail}");

            // Helps some clients show it properly
            sb.AppendLine("STATUS:CONFIRMED");

            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:VCALENDAR");

            return Encoding.UTF8.GetBytes(sb.ToString());

            static string Escape(string value) =>
                value.Replace(@"\", @"\\")
                     .Replace(";", @"\;")
                     .Replace(",", @"\,")
                     .Replace("\n", "\\n");
        }
    }
}
