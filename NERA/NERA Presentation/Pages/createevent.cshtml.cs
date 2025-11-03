using Domain.Entities;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NERA_Presentation.Pages
{
    public class CreateEventModel : PageModel
    {
        private readonly CreateEventService _service;
        private readonly UpdateEventService _updateService;


        public CreateEventModel(CreateEventService service, UpdateEventService updateEventService)
        {
            _service = service;
            _updateService = updateEventService;
        }


        [BindProperty]
        public Event Event { get; set; } = new Event();
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!id.HasValue)
            {
                Event = new Event();
                return Page();
            }
            var exisisting = await _updateService.GetByIdAsync(id.Value);
            if (exisisting == null)
            {
                return NotFound();
            }
            // Populate the page model so the form is pre-filled
            Event = exisisting;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (Event.Id > 0)
            {
                await _updateService.UpdateEventAsync(Event);
            }
            else
            {
                await _service.CreateEventAsync(Event);
            }

            return RedirectToPage("Index");
        }

    }
}