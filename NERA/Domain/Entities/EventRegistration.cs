
using Domain.Enums;

namespace Domain.Entities
{
    public class EventRegistration
    {

        public int Id { get; set; }

        public int EventId { get; set; }
        public required Event Event { get; set; }

        public string UserSub { get; set; } = default!;

        public DateTime RegisteredAtUtc { get; set; } = DateTime.UtcNow;
        public RegistrationStatus Status { get; set; } = RegistrationStatus.Unknown;
        
    }
}
