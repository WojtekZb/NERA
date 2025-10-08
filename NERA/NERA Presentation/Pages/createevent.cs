using Domain.Entities;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NERA_Presentation.Pages
{
    public class Index1Model : PageModel
    {
        private readonly CreateEventService _service;

        public Index1Model(CreateEventService service)
        {
            _service = service;
        }

        [BindProperty]
        public Event Event { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            await _service.CreateEventAsync(Event);
            return RedirectToPage("Index");
        }

    }
}