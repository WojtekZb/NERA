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
        public async Task OnGetAsync()
        {
            Events = await _context.Events
                .AsNoTracking()
                .OrderBy(e => e.StartDate)
                .ToListAsync();

        }
    }
}