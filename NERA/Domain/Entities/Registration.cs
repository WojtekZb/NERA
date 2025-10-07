
namespace Domain.Entities
{
    public class Registration
    {
        public int UserId { get; set; }
        public required User User { get; set; }
        public int EventId { get; set; }
        public required Event Event { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime Time { get; set; }
    }
}
