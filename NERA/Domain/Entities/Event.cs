namespace Domain.Entities
{
    public class Event
    {
        public int Id { get; private set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public double Cost { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; } = string.Empty;

        public ICollection<Registration> Registration { get; set; } = new List<Registration>();
    }
}
