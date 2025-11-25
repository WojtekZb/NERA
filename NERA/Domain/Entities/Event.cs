using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string CGI { get; set; } = string.Empty;
        [Required]
        public string Adress { get; set; } = string.Empty;
        [Required]
        public double Cost { get; set; }
        [Required]
        public int? Capacity { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public EventStatus Status { get; set; }
        //public string OrganizerEmail { get; set; } = default;
        //public string OrganizerName { get; set; } = default;

        public ICollection<EventRegistration> EventRegistration { get; set; } = new List<EventRegistration>();
    }
}
