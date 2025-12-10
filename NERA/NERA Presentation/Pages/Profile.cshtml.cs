using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace NERA_Presentation.Pages
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        public bool IsAdmin { get; private set; }

        public void OnGet()
        {
            // Check if user has Admin role
            IsAdmin = User.IsInRole("Admin") || 
                     User.HasClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin") ||
                     User.Claims.Any(c => (c.Type == "roles" || c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role") && c.Value == "Admin");
        }
    }
}
