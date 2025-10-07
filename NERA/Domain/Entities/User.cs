namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        
        public ICollection<Registration> Registration { get; set; } = new List<Registration>();
    }
}
