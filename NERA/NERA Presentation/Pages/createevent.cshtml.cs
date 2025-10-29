using Domain.Entities;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NERA_Presentation.Pages
{
    public class CreateEventModel : PageModel
    {
        private readonly CreateEventService _service;


        public CreateEventModel(CreateEventService service)
        {
            _service = service;
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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            else
            {
                await _service.CreateEventAsync(Event);
            }

            return RedirectToPage("Index");
        }

    }
}