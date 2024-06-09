using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NguyenMinhNguyen_Web.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Remove("AccountID");
            HttpContext.Session.Remove("RoleID");
            HttpContext.Session.Remove("Token");
            return RedirectToPage("Login");
        }
    }
}
