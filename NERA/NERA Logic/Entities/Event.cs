namespace NERA_Logic.Entities
{
    public class Event
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public double Cost { get; set; }
        public int MaxPeople { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
