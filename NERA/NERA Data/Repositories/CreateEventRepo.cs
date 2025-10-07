using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return evenement;
        }
    }
}
