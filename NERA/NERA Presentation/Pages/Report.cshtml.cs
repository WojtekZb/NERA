using Data;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace NERA_Presentation.Pages
{
    [Authorize]
    public class ReportModel : PageModel
    {
        private readonly AppDbContext _context;

        public ReportModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Event> Events { get; set; } = new List<Event>();
        public bool DbAvailable { get; private set; }

        public async Task OnGetAsync()
        {
            DbAvailable = DbStatus.DbAvailable;

            if (!DbAvailable)
            {
                Events = new List<Event>();
                return;
            }

            try
            {
                Events = await _context.Event
                    .AsNoTracking()
                    .OrderBy(e => e.StartDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load events: {ex.Message}");
                DbAvailable = false;
                Events = new List<Event>();
            }
        }
    }
}

