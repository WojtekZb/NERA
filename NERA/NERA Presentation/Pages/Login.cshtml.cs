using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NERA_Presentation.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        public IActionResult OnGet(string? returnUrl = "/")
        {
            var props = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
            {
                RedirectUri = returnUrl ?? "/"
            };
            return Challenge(props, Auth0Constants.AuthenticationScheme);
        }
    }
}

