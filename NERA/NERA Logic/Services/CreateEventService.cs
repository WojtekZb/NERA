using Domain.Entities;
using Domain.Interfaces;
using System.Text;

namespace Logic.Services
{
    public class CreateEventService
    {
        private readonly ICreateEventRepo _repo;
        private readonly IEmailSender _emailSender;

        public CreateEventService(ICreateEventRepo repo, IEmailSender emailSender)
        {
            _repo = repo;
            _emailSender = emailSender;
        }

        public async Task<Event> CreateEventAsync(Event ev)
        {
            var evenement = new Event
            {
                Id = ev.Id,
                Title = ev.Title,
                StartDate = ev.StartDate,
                EndDate = ev.EndDate,
                Description = ev.Description,
                Cost = ev.Cost,
                Capacity = ev.Capacity,
                CGI = ev.CGI,
                Adress = ev.Adress,
                Status = ev.Status
            };
            return await _repo.SaveAsync(evenement);
        }
        public async Task RegisterUserAsync(string userId, string userEmail, string userName, int eventId)
        {
            // 1. Register in DB
            await _repo.RegisterUserToEventAsync(userId, eventId);

            // 2. Get event data
            var ev = await _repo.GetByIdAsync(eventId)
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
            sb.AppendLine($"UID:{Guid.NewGuid()}");
            sb.AppendLine($"DTSTAMP:{FormatUtc(DateTime.UtcNow)}");
            sb.AppendLine($"DTSTART:{FormatUtc(ev.StartDate)}");
            sb.AppendLine($"DTEND:{FormatUtc(ev.EndDate)}");
            sb.AppendLine($"SUMMARY:{Escape(ev.Title)}");
            if (!string.IsNullOrWhiteSpace(ev.Description))
                sb.AppendLine($"DESCRIPTION:{Escape(ev.Description!)}");
            if (!string.IsNullOrWhiteSpace(ev.Adress))
                sb.AppendLine($"LOCATION:{Escape(ev.Adress!)}");

            sb.AppendLine($"ORGANIZER;CN=\"{Escape(ev.OrganizerName)}\":mailto:{ev.OrganizerEmail}");
            sb.AppendLine($"ATTENDEE;CN=\"{Escape(attendeeName)}\";RSVP=TRUE:mailto:{attendeeEmail}");

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