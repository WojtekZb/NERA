using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

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
        public async Task UpdateEventAsync(Event evenement)
        {
            //Load the trackes entity
            var existing = await _context.Events.FindAsync(evenement.Id);
            if(existing == null)
            {
                throw new KeyNotFoundException($"Event with Id{evenement.Id} not found");
            }
            //copy editable fields
            existing.Title = evenement.Title;
            existing.StartDate = evenement.StartDate;
            existing.EndDate = evenement.EndDate;
            existing.Location = evenement.Location;
            existing.Cost = evenement.Cost;
            existing.Capacity = evenement.Capacity;
            existing.Description = evenement.Description;
            existing.Status = evenement.Status;

            //save changes
            await _context.SaveChangesAsync();
            Console.WriteLine("Updating event in DB: " + existing.Title);
        }
        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e=> e.Id == id);
        }

        public async Task<Event> SaveChangeAsync(Event evenement)
        {
            // For detached entities: attach and mark modified
            _context.Attach(evenement);
            _context.Entry(evenement).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return evenement;
        }
    }
}
