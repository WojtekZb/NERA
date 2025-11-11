using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NERA_Presentation.Pages
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly IConfiguration _config;
        public LogoutModel(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IActionResult> OnGet()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = _config["Auth0:PostLogoutRedirectUri"] ?? Url.Content("~/")
            };

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, props);

            return new EmptyResult();
        }
    }
}
