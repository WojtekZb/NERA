using Domain.Entities;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NERA_Presentation.Pages
{
    // Presentation/Pages/Events/Create.cshtml.cs
    public class Index1Model : PageModel
    {
        private readonly CreateEventService _service;

        public Index1Model(CreateEventService service)
        {
            _service = service;
        }

        [BindProperty]
        public Event EventDto { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var createdEvent = await _service.CreateEventAsync(EventDto);
            return RedirectToPage("Details", new { id = createdEvent.Id });
        }
    }
}
