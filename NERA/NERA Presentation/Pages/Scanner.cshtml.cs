using Domain.Interfaces;
using Logic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NERA_Presentation.Pages
{
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = "Admin")]
    public class ScannerModel : PageModel
    {
        private readonly RegisterUserToEventService _service;

        public ScannerModel(RegisterUserToEventService service)
        {
            _service = service;
        }

        // This handler will be called from JS
        public async Task<IActionResult> OnPostChangeAttendance([FromBody] QrPayloadDto dto)
        {
            await _service.ChangeAttandance(dto.Qr);
            return new JsonResult(new { success = true });
        }

    }

    public record QrPayloadDto(string Qr);

}

