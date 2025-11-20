
using Domain.Enums;

namespace Domain.Entities
{
    public class EventRegistration
    {

        public int Id { get; set; }

        public int EventId { get; set; }

        public string UserSub { get; set; } = default!;
    }
}
