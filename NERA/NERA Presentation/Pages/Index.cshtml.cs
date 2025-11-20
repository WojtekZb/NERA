using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Data;
using Logic.Services;
using System.Text.Json.Serialization;
using System.Security.Claims;
namespace NERA_Presentation.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly RegisterUserToEventService _registerService;

        public IndexModel(AppDbContext context, RegisterUserToEventService registerService)
        {
            _context = context;
            _registerService = registerService;
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
                Events = await _context.Event
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
            var eventToDelete = await _context.Event.FindAsync(id);

            if (eventToDelete != null)
            {
                _context.Event.Remove(eventToDelete);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public class RegisterRequest
        {
            [JsonPropertyName("EventId")]
            public int EventId { get; set; }
        }

        public async Task<IActionResult> OnPostRegisterAsync([FromBody] RegisterRequest data)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            //var userName = User.FindFirst(ClaimTypes.Name).Value;
            if (userId == null)
                return Unauthorized();

            await _registerService.RegisterForEvent(userId, userEmail, "Wojtek", data.EventId);

            return new JsonResult(new { success = true });
        }
    }
}