using Domain.Entities;
using Domain.Interfaces;

namespace Data.Repositories
{
    public class CreateEventRepo : ICreateEventRepo
    {
        private readonly AppDbContext _context;

        public CreateEventRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Event> SaveAsync(Event evenement)
        {
            _context.Events.Add(evenement);
            await _context.SaveChangesAsync();
            Console.WriteLine("Saving event to DB: " + evenement.Title);
            return evenement;
        }
    }
}
