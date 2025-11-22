using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Data;
using Logic.Services;
using Microsoft.AspNetCore.Identity.Data;
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
            if (userId == null)
                return Unauthorized();

            await _registerService.RegisterForEvent(userId, data.EventId);

            return new JsonResult(new { success = true });
        }

        public async Task<IActionResult> OnGetUserEventsAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return new JsonResult(new { events = new List<object>() });

            if (!DbStatus.DbAvailable)
            {
                return new JsonResult(new { events = new List<object>() });
            }

            try
            {
                var userEvents = await _context.EventRegistration
                    .Where(er => er.UserSub == userId)
                    .Join(_context.Event,
                        registration => registration.EventId,
                        evt => evt.Id,
                        (registration, evt) => evt)
                    .AsNoTracking()
                    .Select(e => new
                    {
                        id = e.Id,
                        title = e.Title,
                        startDate = e.StartDate,
                        endDate = e.EndDate,
                        cgi = e.CGI,
                        adress = e.Adress,
                        status = e.Status.ToString(),
                        cost = e.Cost,
                        capacity = e.Capacity,
                        description = e.Description
                    })
                    .ToListAsync();

                return new JsonResult(new { events = userEvents });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user events: {ex.Message}");
                return new JsonResult(new { events = new List<object>() });
            }
        }
    }
}