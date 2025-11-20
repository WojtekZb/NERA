using Logic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NERA_Presentation.Pages
{
    [Authorize]
    public class RegisterModel : PageModel
    {
        private readonly CreateEventService _eventService;
        public RegisterModel(CreateEventService eventService)
        {
            _eventService = eventService;
        }
        [BindProperty(SupportsGet = true)]
        public int EventId { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirst("sub")?.Value ?? User.Identity!.Name!;
            var userEmail = User.Identity!.Name!;           // or from claims
            var userName = User.Identity!.Name!;            // or full name claim

            await _eventService.RegisterUserAsync(userId, userEmail, userName, EventId);

            TempData["Message"] = "You are registered. A confirmation email has been sent.";
            return RedirectToPage("/Pages/Index", new { id = EventId });
        }
    }
}
