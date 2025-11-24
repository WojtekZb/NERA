using Data;
using Domain.Entities;
using Logic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json.Serialization;
namespace NERA_Presentation.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly RegisterUserToEventService _registerService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(AppDbContext context, RegisterUserToEventService registerService, ILogger<IndexModel> logger)
        {
            _context = context;
            _registerService = registerService;
            _logger = logger;
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
            // Auth0-style claims
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return new JsonResult(new { success = false, error = "not_authenticated" })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }

            // Auth0 claims – adjust if you changed mapping
            var userId =
                User.FindFirstValue("sub") ??
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userEmail =
                User.FindFirstValue("email") ??
                User.FindFirstValue(ClaimTypes.Email);

            var userName =
                User.FindFirstValue("name") ??
                User.FindFirstValue(ClaimTypes.Name) ??
                userEmail;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
            {
                _logger.LogWarning("Missing claims for registration. sub={Sub}, email={Email}",
                    userId, userEmail);

                return new JsonResult(new { success = false, error = "missing_claims" })
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            try
            {
                await _registerService.RegisterForEvent(userId, userEmail, userName!, data.EventId);

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error while registering user {UserId} for event {EventId}", userId, data.EventId);

                return new JsonResult(new { success = false, error = "server_error" })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}