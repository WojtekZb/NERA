using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Event
    {
        public int Id { get; private set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Location { get; set; } = string.Empty;
        [Required]
        public double Cost { get; set; }
        [Required]
        public int Capacity { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;

        public ICollection<Registration> Registration { get; set; } = new List<Registration>();
    }
}
