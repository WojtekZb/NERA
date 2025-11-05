using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Data;
namespace NERA_Presentation.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }
        public IList<Event> Events { get; private set; } = new List<Event>();
        public bool DbAvailable { get; private set; }

        public async Task OnGetAsync()
        {
            DbAvailable = DbStatus.DbAvailable;

            if (!DbAvailable)
            {
                // DB not reachable — skip querying and show limited page
                Console.WriteLine("?? Database unavailable — loading homepage in limited mode.");
                Events = new List<Event>();
                return;
            }

            try
            {
                Events = await _context.Events
                    .AsNoTracking()
                    .OrderBy(e => e.StartDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // If the DB goes down mid-request, handle gracefully
                Console.WriteLine("?? Failed to load events: " + ex.Message);
                DbAvailable = false;
                Events = new List<Event>();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var eventToDelete = await _context.Events.FindAsync(id);

            if (eventToDelete != null)
            {
                _context.Events.Remove(eventToDelete);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}