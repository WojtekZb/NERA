using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NERA_Presentation.Pages
{
    [Authorize]
    public class ScannerModel : PageModel
    {
        private readonly ILogger<ScannerModel> _logger;

        public ScannerModel(ILogger<ScannerModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}

